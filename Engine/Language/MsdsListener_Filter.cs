using System.Linq;
using Antlr4.Runtime.Misc;

namespace Engine.Language
{
    partial class MsdsListener
    {
        public override void ExitFilter_collection([NotNull] MsdsParser.Filter_collectionContext context)
        {
            var collectionId = context.collectionid().GetText();
            var sql = $"@collectionId = '{collectionId}'";
            _sql.Put(context, sql);
        }

        public override void ExitFilter_collections([NotNull] MsdsParser.Filter_collectionsContext context)
        {
            var data = new
            {
                collectionIds = context.collectionid().Select(x => x.GetText())
            };
            var ruleSql = _engine.Generate("FilterCollections", data);
            _sql.Put(context, ruleSql);
        }

        public override void ExitFilter_operation([NotNull] MsdsParser.Filter_operationContext context)
        {
            var filter1 = _sql.Get(context.filter(0));
            var filter2 = _sql.Get(context.filter(1));
            var op = context.operation().GetText().ToUpper();
            _sql.Put(context, $"{filter1} {op} {filter2}");
        }
    }
}
