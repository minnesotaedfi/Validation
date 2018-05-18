using Antlr4.Runtime.Tree;

namespace Engine.Language
{
    public class ParseTreeProperty<T> : Antlr4.Runtime.Tree.ParseTreeProperty<T>
    {
        public override T Get(IParseTree node)
        {
            return node == null? default(T): base.Get(node);
        }
    }
}
