using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Engine.Models;
using Engine.Templates;

namespace Engine.Language
{
    public partial class MsdsListener : MsdsParserBaseListener
    {
        // Storage for the generated SQL text
        private readonly ParseTreeProperty<List<Component>> _components = new ParseTreeProperty<List<Component>>();
        private readonly ParseTreeProperty<List<string>> _collectionIds = new ParseTreeProperty<List<string>>();
        private readonly ParseTreeProperty<string> _sql = new ParseTreeProperty<string>();
        private readonly ParseTreeProperty<object> _value = new ParseTreeProperty<object>();
        private readonly TemplateEngine _engine;
        private string _currentRulesetId;
        private int _temporary = 0;

        public Model Model { get; }

        public IDateProvider DateProvider { get; set; } = new DateProvider();
        public IDateCalculator DateCalculator { get; set; } = new DateCalculator(Month.July, 1);
        public ISchemaProvider SchemaProvider { get; set; } = new SchemaProvider();

        public MsdsListener(Model model = null, TemplateEngine engine = null)
        {
            Model = model ?? new Model();
            _engine = engine ?? new TemplateEngine();
        }

        public override void ExitCollection([NotNull] MsdsParser.CollectionContext context)
        {
            var collection = new Collection(context.collectionid().ID().GetText());
            foreach (var rulesetid in context.rulesetid()) collection.AddRulesetReference(rulesetid.GetText());
            foreach (var ruleid in context.ruleid()) collection.AddRuleReference(ruleid.GetText());
            Model.AddCollection(collection);
            //_aliasRegistry.Put(context, collection);
        }

        public override void ExitFunction([NotNull] MsdsParser.FunctionContext context)
        {
            Model.AddFunction(context.functionid().GetText());
        }

        public override void ExitRulesetid([NotNull] MsdsParser.RulesetidContext context)
        {
            _currentRulesetId = context.GetText();
        }

        public override void ExitRuleset([NotNull] MsdsParser.RulesetContext context)
        {
            _currentRulesetId = null;
        }

        public override void ExitEveryRule([NotNull] ParserRuleContext context)
        {
            CollectComponents(context);
            CollectSql(context);
            CollectCollectionIds(context);
        }

        private void CollectSql(ParserRuleContext context)
        {
            if (_sql.Get(context) == null 
                && context.children != null 
                && context.children.Count == 1 
                && !string.IsNullOrEmpty(_sql.Get(context.children[0])))
            {
                _sql.Put(context, _sql.Get(context.children[0]));
            }
        }
    }
}
