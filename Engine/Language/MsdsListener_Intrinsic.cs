using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using Engine.Models;

namespace Engine.Language
{
    public partial class MsdsListener
    {
        private string _tableSuffix;

        /// <summary>
        /// used to compute the units of time since an event happened (i.e. birthday)
        /// </summary>
        public override void ExitIntrinsic_timePeriod([NotNull] MsdsParser.Intrinsic_timePeriodContext context)
        {
            var today = DateProvider.Today.ToString(SqlDateFormat);
            var data = new
            {
                datepart = context.TIMEUNIT().GetText().TrimEnd(new char[] { 's' }),
                startdate = _sql.Get(context.date()[0]),
                enddate = context.date().Length != 2 ? $"'{today}'" : _sql.Get(context.date()[1])
            };
            var ruleSql = _engine.Generate("IntrinsicTimePeriod", data);
            _sql.Put(context, ruleSql);
        }

        public override void EnterIntrinsic_aggregatecount([NotNull] MsdsParser.Intrinsic_aggregatecountContext context)
        {
            _temporary++;
            _tableSuffix = $"_{_temporary}";
        }

        public override void ExitIntrinsic_aggregatecount([NotNull] MsdsParser.Intrinsic_aggregatecountContext context)
        {
            var componentName = context.componentid().ID().GetText();
            if (context.characteristicids() != null)
            {
                var tmpComponents = new List<Component>();
                tmpComponents.AddRange(context.characteristicids().ID().Select(x => new Component(componentName, x.GetText())));
                _components.Put(context, tmpComponents);
            }

            var components = AssembleChildComponents(context);

            var data = new
            {
                schema = SchemaProvider.Value,
                componentName,
                characteristicIds = context.characteristicids()?.ID().Select(x => x.GetText()),
                tableSuffix = _tableSuffix,
                tables = AssembleChildComponents(context).Select(x => x.ComponentName).Distinct().ToList(),
                sqlwhere = _sql.Get(context.condition())
            };
            var ruleSql = _engine.Generate("IntrinsicCount", data);
            _sql.Put(context, ruleSql);
            _tableSuffix = string.Empty;
        }

        public override void EnterIntrinsic_aggregate([NotNull] MsdsParser.Intrinsic_aggregateContext context)
        {
            _temporary++;
            _tableSuffix = $"_{_temporary}";
        }

        public override void ExitIntrinsic_aggregate([NotNull] MsdsParser.Intrinsic_aggregateContext context)
        {
            var component = _components.Get(context.component())[0];
            var data = new
            {
                schema = SchemaProvider.Value,
                componentName = component.ComponentName,
                characteristicName = component.CharacteristicName,
                characteristicIds = context.characteristicids()?.ID().Select(x => x.GetText()),
                operation = context.AGGREGATE().GetText().ToUpper(),
                tables = AssembleChildComponents(context).Select(x => x.ComponentName).Distinct().ToList(),
                tableSuffix = _tableSuffix,
                sqlwhere = _sql.Get(context.condition())
            };
            var ruleSql = _engine.Generate("IntrinsicAggregate", data);
            _sql.Put(context, ruleSql);
            _tableSuffix = string.Empty;
        }

        public override void ExitIntrinsic_arithmetic([NotNull] MsdsParser.Intrinsic_arithmeticContext context)
        {
            var data = new
            {
                operation = context.AGGREGATE().GetText().ToUpper(),
                components = AssembleChildComponents(context)
            };
            var ruleSql = _engine.Generate("IntrinsicArithmetic", data);
            _sql.Put(context, ruleSql);
        }
    }
}
