using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace Engine.Language
{
    public partial class MsdsListener
    {
        public override void ExitCollectionid([NotNull] MsdsParser.CollectionidContext context)
        {
            _collectionIds.Put(context, new List<string> { context.ID().GetText() });
        }

        private void CollectCollectionIds(ParserRuleContext context)
        {
            var collections = AssembleChildCollectionIds(context);
            if (collections.Count > 0)
            {
                _collectionIds.Put(context, collections);
            }
        }

        private List<string> AssembleChildCollectionIds(ParserRuleContext context)
        {
            var collectionIds = new List<string>();
            if (context.children != null)
                foreach (var childCollectionIds in context.children.Select(ctx => _collectionIds.Get(ctx)))
                {
                    collectionIds.AddRange(childCollectionIds ?? new List<string>());
                }
            return collectionIds.Distinct().ToList();
        }
    }
}
