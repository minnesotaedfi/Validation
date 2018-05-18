using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using Engine.Models;

namespace Engine.Language
{
    public partial class MsdsListener
    {
        public override void ExitCondition_comparison([NotNull] MsdsParser.Condition_comparisonContext context)
        {
            var data = new
            {
                expr1 = _sql.Get(context.expr()[0]),
                expr2 = _sql.Get(context.expr()[1]),
                comparison = (context.comparison().LT() == null ? string.Empty : "<")
                    + (context.comparison().LE() == null ? string.Empty : "<=")
                    + (context.comparison().EQ() == null ? string.Empty : "=")
                    + (context.comparison().GE() == null ? string.Empty : ">=")
                    + (context.comparison().GT() == null ? string.Empty : ">")
                    + (context.comparison().NE() == null ? string.Empty : "<>")
            };
            var sql = _engine.Generate("ConditionInComparison", data);
            _sql.Put(context, sql);
        }

        public override void ExitCondition_parenthesis([NotNull] MsdsParser.Condition_parenthesisContext context)
        {
            var condition = _sql.Get(context.condition());
            _sql.Put(context, $"({condition})");
        }

        public override void ExitCondition_compound([NotNull] MsdsParser.Condition_compoundContext context)
        {
            var condition1 = _sql.Get(context.condition(0));
            var condition2 = _sql.Get(context.condition(1));
            var op = context.operation(0).GetText().ToUpper();
            _sql.Put(context, $"{condition1} {op} {condition2}");
        }

        public override void ExitCondition_pattern([NotNull] MsdsParser.Condition_patternContext context)
        {
            var component = _sql.Get(context.component());
            var not = context.NOT() == null ? "" : "NOT";
            var pattern = context.pattern().GetText();
            _sql.Put(context, $"{component} {not} LIKE {pattern}");
        }

        public override void ExitCondition_inconsts([NotNull] MsdsParser.Condition_inconstsContext context)
        {
            var component = _sql.Get(context.component());
            var not = context.NOT() == null ? "" : "NOT";
            var consts = _sql.Get(context.constants());
            _sql.Put(context, $"{component} {not} IN {consts}");
        }

        public override void ExitCondition_intuples([NotNull] MsdsParser.Condition_intuplesContext context)
        {
            var data = new
            {
                tableName = $"T{_temporary++}",
                tuples = _sql.Get(context.tuples()),
                components = _components.Get(context.components()).Where(x => !string.IsNullOrEmpty(x.CharacteristicName))
            };
            var sql = _engine.Generate("ConditionInTuples", data);
            _sql.Put(context, sql);
        }

        public override void ExitCondition_inlookups([NotNull] MsdsParser.Condition_inlookupsContext context)
        {
            var data = new
            {
                schema = SchemaProvider.Value,
                tableName = $"_{_temporary++}",
                components = _components.Get(context.components()).Where(x=> !string.IsNullOrEmpty(x.CharacteristicName)),
                lookupTableName = context.lookups().componentid().ID().GetText(),
                lookups = context.lookups().characteristicids().ID().Select(id => id.GetText()).ToList()
            };
            var sql = _engine.Generate("ConditionInLookups", data);
            _sql.Put(context, sql);
        }

        public override void ExitCondition_exists1([NotNull] MsdsParser.Condition_exists1Context context)
        {
            var component = _sql.Get(context.component());
            var @null = context.NOT() == null ? "NOT NULL" : "NULL";
            var sql = $"{component} IS {@null}";
            _sql.Put(context, sql);
        }

        public override void ExitCondition_exists2([NotNull] MsdsParser.Condition_exists2Context context)
        {
            // This is an unusual case to add a component, although we only have a componentName, not characteristicName
            var component = new Component(context.componentid().ID().GetText());
            _components.Put(context, new List<Component> { component });
            Model.AddComponent(component.ComponentName);

            var @null = context.NOT() == null ? "NOT NULL" : "NULL";
            var sql = $"[{SchemaProvider.Value}].[{component.ComponentName}].[Id] IS {@null}";
            _sql.Put(context, sql);
        }

        public override void ExitCondition_unique([NotNull] MsdsParser.Condition_uniqueContext context)
        {
            var componentName = _components.Get(context.components()).FirstOrDefault().ComponentName;
            var characteristicName = _components.Get(context.components()).Select(x => x.CharacteristicName);
            var data = new
            {
                schema = SchemaProvider.Value,
                componentName,
                characteristicName,
                tableName = $"T{_temporary++}"
            };
            var sql = _engine.Generate("ConditionUnique", data);
            _sql.Put(context, sql);
        }
    }
}
