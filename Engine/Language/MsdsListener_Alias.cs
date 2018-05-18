using System.Data.SqlClient;
using Antlr4.Runtime.Misc;

namespace Engine.Language
{
    public partial class MsdsListener
    {
        public override void ExitAlias([NotNull] MsdsParser.AliasContext context)
        {
            var collectionId = string.Empty;
            var parent = context.parent.parent as MsdsParser.CollectionContext;
            if (parent != null)
            {
                collectionId = parent.collectionid().GetText();
            }
            var parameterName = $"@{context.aliasId().GetText()}";
            var value = _value.Get(context.constant());
            Model.AddParameter(collectionId, new SqlParameter(parameterName, value));
        }

        public override void ExitAliasId([NotNull] MsdsParser.AliasIdContext context)
        {
            var aliasId = context.GetText();
            _sql.Put(context, $"@{aliasId}");
        }
    }
}
