using System.Linq;
using Antlr4.Runtime.Misc;

namespace Engine.Language
{
    public partial class MsdsListener
    {
        public override void ExitBool([NotNull] MsdsParser.BoolContext context)
        {
            var sql = (context.TRUE() == null ? string.Empty : "1")
                    + (context.FALSE() == null ? string.Empty : "0");
            _sql.Put(context, sql);
            _value.Put(context, sql == "1");
        }

        public override void ExitNum([NotNull] MsdsParser.NumContext context)
        {
            var strValue = context.GetText();
            _value.Put(context, decimal.Parse(strValue));
            _sql.Put(context, strValue);
        }

        public override void ExitStr([NotNull] MsdsParser.StrContext context)
        {
            var strValue = context.STRING().GetText();
            _value.Put(context, strValue);
            var strEscaped = strValue.Replace(@"\'", "''");
            _sql.Put(context, strEscaped);
        }

        public override void ExitConstant([NotNull] MsdsParser.ConstantContext context)
        {
            var value = _value.Get(context.GetChild(0));
            _value.Put(context, value);
        }

        public override void ExitConstants([NotNull] MsdsParser.ConstantsContext context)
        {
            var values = string.Join(", ", context.constant().Select(x => _sql.Get(x)));
            _sql.Put(context, $"({values})");
        }

        public override void ExitTuple([NotNull] MsdsParser.TupleContext context)
        {
            var values = string.Join(", ", context.constant().Select(x => _sql.Get(x)));
            _sql.Put(context, $"({values})");
        }

        public override void ExitTuples([NotNull] MsdsParser.TuplesContext context)
        {
            var values = string.Join(", ", context.tuple().Select(x => _sql.Get(x)));
            _sql.Put(context, $"{values}");
        }
    }
}
