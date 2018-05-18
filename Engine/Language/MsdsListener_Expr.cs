using Antlr4.Runtime.Misc;
using System.Linq;

namespace Engine.Language
{
    public partial class MsdsListener
    {
        public override void ExitExpr_component([NotNull] MsdsParser.Expr_componentContext context)
        {
            var componentId = context.component().componentid().ID().GetText();
            var characteristicId = context.component().characteristicid().ID().GetText();
            _sql.Put(context, $"[{componentId}{_tableSuffix}].[{characteristicId}]");
        }

        public override void ExitExpr_function([NotNull] MsdsParser.Expr_functionContext context)
        {
            var data = new
            {
                schema = SchemaProvider.Value,
                name = context.function().functionid().GetText(),
                parameters = context.function().expr().Select(expr => _sql.Get(expr))
                    .Where(x => !string.IsNullOrEmpty(x.ToString())).ToList()
            };
            var sql = _engine.Generate("function", data);
            _sql.Put(context, sql);
        }
    }
}
