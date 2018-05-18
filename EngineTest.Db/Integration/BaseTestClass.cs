using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Engine.Language;
using Engine.Models;
using Engine.Utility;

namespace EngineTest.Db.Integration
{
    public abstract class BaseTestClass
    {
        private List<RuleErrors> _ruleErrors;

        protected virtual string CollectionName { get; set; } = string.Empty;
        protected abstract string SetRules();
        protected abstract void SetTestData(List<Component1> component1, List<Component2> component2);

        public IReadOnlyList<RuleErrors> RuleErrors => _ruleErrors;

        public IDateProvider DateProvider { get; } = new StaticDateProvider(new DateTime(2000, 7, 1));

        [TestInitialize]
        public void TestInitialize()
        {
            using (var context = new EngineDbTestContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    DoPopulateTestData(context);

                    var stream = SetRules().ToStream();
                    var builder = new ModelBuilder(stream);
                    var model = builder.Build(DateProvider);

                    _ruleErrors = DoRunRules(context, model.Rules.Select(x => x.Sql), model.GetParameters(CollectionName));
                }
                finally
                {
                    transaction.Rollback();
                }
            }
        }

        private List<RuleErrors> DoRunRules(DbContext context, IEnumerable<string> sqlRules, params SqlParameter[] parameters)
        {
            var results = new List<RuleErrors>();
            foreach (var sql in sqlRules)
            {
                Console.WriteLine(sql);
                var @params = parameters.Select(x => new SqlParameter(x.ParameterName, x.Value)).ToArray();
                // ReSharper disable once CoVariantArrayConversion
                var query = context.Database.SqlQuery<RuleErrors>(sql, @params);
                results.AddRange(query.ToList());
            }
            return results;
        }

        private void DoPopulateTestData(EngineDbTestContext context)
        {
            var component1 = new List<Component1>();
            var component2 = new List<Component2>();
            SetTestData(component1, component2);
            context.Component1S.AddRange(component1);
            context.Component2S.AddRange(component2);
            context.SaveChanges();
        }
    }
}
