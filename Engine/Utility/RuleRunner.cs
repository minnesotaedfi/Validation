using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Engine.Language;
using Engine.Models;
using log4net;
using System.Data.Entity.Infrastructure;

namespace Engine.Utility
{
    public class RuleRunner
    {
        private readonly Model _model;
        private readonly ISchemaProvider _schema;

        public RuleRunner(Model model, ISchemaProvider schema)
        {
            _model = model;
            _schema = schema;
        }

        private static ILog Log => LogManager.GetLogger(typeof(RuleRunner));

        /// <summary>
        /// Run the rules without storing the results in the database
        /// </summary>
        public async Task<bool> Test(string connectionString, string collectionId)
        {
            try
            {
                var actionBlock = new ActionBlock<Rule>(r => TestRule(r, connectionString, collectionId),
                    new ExecutionDataflowBlockOptions
                    {
                        BoundedCapacity = 5,
                        MaxDegreeOfParallelism = 5
                    });

                var bufferBlock = new BufferBlock<Rule>();

                bufferBlock.LinkTo(actionBlock);

                try
                {
                    foreach (var rule in _model.GetRules(collectionId))
                    {
                        using (NDC.Push(rule.RuleId))
                        {
                            Log.Info("test pending");
                            Log.Debug(rule.Sql);
                        }
                        bufferBlock.Post(rule);
                    }
                    bufferBlock.Complete();
                    await actionBlock.Completion;
                }
                catch (AggregateException ex)
                {
                    foreach (var innerException in ex.InnerExceptions)
                    {
                        Log.Fatal(ex);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex);
                return false;
            }
            return true;
        }

        private async Task TestRule(Rule rule, string connectionString, string collectionId)
        {
            using (NDC.Push(rule.RuleId))
            {
                Log.Warn("test started");
                Log.Debug(rule.Sql);
            }
            var @params = _model.GetParameters(collectionId).Select(x => new SqlParameter(x.ParameterName, x.Value)).ToArray();
            try
            {
                long count;
                using (var dbContext = new DbContext(connectionString))
                {
                    // ReSharper disable once CoVariantArrayConversion
                    var results = await dbContext.Database.SqlQuery<RuleErrors>(rule.Sql, @params).ToListAsync();
                    count = results.LongCount();
                }
                using (NDC.Push(rule.RuleId))
                    Log.Info($"completed with {count} errors");
            }
            catch (Exception ex)
            {
                using (NDC.Push(rule.RuleId)) Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Run the rules and commit the results to the database
        /// </summary>
        public async Task<bool> Run(string connectionString, string collectionId)
        {
            try
            {
                var validationId = await GetValidationId(connectionString, collectionId);

                var rules = _model.GetRules(collectionId).ToArray();

                PopulateRuleIdToComponentTable(rules, connectionString, validationId);

                int numberOfRulesFailed = 0;

                var ruleStatusCheckBlock = new ActionBlock<bool>(success => { if (!success) { ++numberOfRulesFailed; } });

                var runRuleBlock = new TransformBlock<Rule, bool>(r => RunRule(r, connectionString, collectionId, validationId),
                    new ExecutionDataflowBlockOptions
                    {
                        BoundedCapacity = 5,
                        MaxDegreeOfParallelism = 5
                    });

                var bufferBlock = new BufferBlock<Rule>();

                bufferBlock.LinkTo(runRuleBlock, new DataflowLinkOptions { PropagateCompletion = true } );
                runRuleBlock.LinkTo(ruleStatusCheckBlock, new DataflowLinkOptions { PropagateCompletion = true });

                try
                {
                    foreach (var rule in _model.GetRules(collectionId))
                    {
                        using (NDC.Push(rule.RuleId))
                        {
                            Log.Info("run pending");
                            Log.Debug(rule.Sql);
                        }
                        bufferBlock.Post(rule);
                    }
                    bufferBlock.Complete();
                    await ruleStatusCheckBlock.Completion;
                }
                catch (AggregateException ex)
                {
                    foreach (var innerException in ex.InnerExceptions)
                    {
                        Log.Fatal(ex);
                    }
                    return false;
                }
                if (numberOfRulesFailed != 0)
                    return false;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex);
                return false;
            }
            return true;
        }
        

        private async Task<long> GetValidationId(string connectionString, string collectionId)
        {
            long result;
            using (var dbContext = new DbContext(connectionString))
            {
                var ruleValidationIdParam =
                    new SqlParameter("RuleValidationId", System.Data.SqlDbType.BigInt)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };

                var collectionIdParam = new SqlParameter("CollectionId", collectionId);

                await dbContext.Database.ExecuteSqlCommandAsync(
                    $"INSERT [{_schema.Value}].[RuleValidation](CollectionId) Values(@CollectionId); " +
                    "SET @RuleValidationId = SCOPE_IDENTITY()",
                    ruleValidationIdParam, collectionIdParam);

                result = (long)ruleValidationIdParam.Value;
            }
            return result;
        }

        private void PopulateRuleIdToComponentTable(IEnumerable<Rule> rules, string connectionString, long validationId)
        {
            var sql = $"INSERT [{_schema.Value}].[RuleValidationRuleComponent](RuleValidationId, RulesetId, RuleId, Component) " +
                               "VALUES (@RuleValidationId, @RulesetId, @RuleId, @Component)";

            var ruleComponents = rules.SelectMany(r => r.Components.Distinct()
                .Select(c => new { r.RulesetId, r.RuleId, Component = c }));

            var tasks = ruleComponents.Select(async x =>
            {
                using (var dbContext = new DbContext(connectionString))
                {
                    var ruleValidationIdParam = new SqlParameter("RuleValidationId", validationId);
                    var rulesetIdParam = new SqlParameter("RulesetId", x.RulesetId);
                    var ruleIdParam = new SqlParameter("RuleId", x.RuleId);
                    var componentParam = new SqlParameter("Component", x.Component);
                    try
                    {
                        return await dbContext.Database.ExecuteSqlCommandAsync(sql,
                            ruleValidationIdParam, rulesetIdParam, ruleIdParam, componentParam);
                    }
                    catch (Exception ex)
                    {
                        using (NDC.Push(x.RuleId))
                        using (NDC.Push(x.Component))
                        {
                            Log.Fatal(ex);
                        }
                        return 0;
                    }
                }
            }).ToArray();

            // ReSharper disable once CoVariantArrayConversion
            Task.WaitAll(tasks);
        }

        private async Task<bool> RunRule(Rule rule, string connectionString, string collectionId, long ruleValidationId)
        {
            using (NDC.Push(rule.RuleId))
            {
                Log.Warn("run started");
                Log.Debug(rule.Sql);
            }
            // ReSharper disable once CoVariantArrayConversion
            using (var dbContext = new DbContext(connectionString))
            {
                try
                {
                    var detailParams = new List<SqlParameter>
                    {
                        new SqlParameter("@RuleValidationId", ruleValidationId)
                    };
                    detailParams.AddRange(
                        _model.GetParameters(collectionId)
                            .Select(x => new SqlParameter(x.ParameterName, x.Value)));
                    dbContext.Database.CommandTimeout = 60;
                    var result = await dbContext.Database.ExecuteSqlCommandAsync(rule.ExecSql, detailParams.ToArray());
                    using (NDC.Push(rule.RuleId)) Log.Info($"completed with {result} errors");
                }
                catch (Exception ex)
                {
                    using (NDC.Push(rule.RuleId)) Log.Error(ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
