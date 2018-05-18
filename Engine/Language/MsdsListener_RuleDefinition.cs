using System;
using Antlr4.Runtime.Misc;
using Engine.Models;
using System.Linq;
using log4net;

namespace Engine.Language
{
    public partial class MsdsListener
    {
        public override void ExitRuleDefinition([NotNull] MsdsParser.RuleDefinitionContext context)
        {
            var ruleId = context.ruleid().GetText();
            try
            {
                var data = new
                {
                    schema = SchemaProvider.Value,
                    ruleid = ruleId,
                    iserror = context.REQUIRE() != null,
                    message = context.error().GetText().Replace(@"\'","''"),
                    tables = AssembleChildComponents(context).Select(x => x.ComponentName).Distinct().ToList(),
                    sqlFilters = _sql.Get(context.filter()),
                    sqlConditions = _sql.Get(context.condition())
                };
                var ruleSql = _engine.Generate("RuleDefinition", data);
                _sql.Put(context, ruleSql);

                var collectionIds = AssembleChildCollectionIds(context);

                var rule = new Rule(context.ruleid().GetText(), _currentRulesetId, SchemaProvider, collectionIds, ruleSql, data.tables);
                Model.AddRule(rule);
            }
            catch (NullReferenceException ex)
            {
                var log = LogManager.GetLogger(this.GetType().ToString());
                using (NDC.Push(ruleId)) log.Fatal(ex);
            }
            catch (ArgumentException ex)
            {
                var log = LogManager.GetLogger(this.GetType().ToString());
                using (NDC.Push(ruleId)) log.Fatal(ex);
            }
        }
    }
}
