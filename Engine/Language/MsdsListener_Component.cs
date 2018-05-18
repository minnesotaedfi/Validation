using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using Engine.Models;
using Antlr4.Runtime;

namespace Engine.Language
{
    public partial class MsdsListener
    {
        public override void ExitComponentid([NotNull] MsdsParser.ComponentidContext context)
        {
            var component = new Component(context.ID().GetText());
            _components.Put(context, new List<Component> { component });

            Model.AddComponent(component.ComponentName);
        }

        public override void ExitComponent([NotNull] MsdsParser.ComponentContext context)
        {
            var component = new Component(context.componentid().ID().GetText(),
                context.characteristicid().ID().GetText());
            _components.Put(context, new List<Component> { component });

            Model.AddComponent(component.ComponentName, component.CharacteristicName);
            _sql.Put(context, $"[{component.ComponentName}].[{component.CharacteristicName}]");
        }

        public override void ExitComponents([NotNull] MsdsParser.ComponentsContext context)
        {
            var components = new List<Component>();
            var componentId = context.componentid().ID().GetText();
            foreach (var id in context.characteristicids().ID())
            {
                components.Add(new Component(componentId, id.GetText()));
                Model.AddComponent(componentId, id.GetText());
            }
            _components.Put(context, components);
        }

        private void CollectComponents(ParserRuleContext context)
        {
            if (_components.Get(context) != null) return;
            var components = AssembleChildComponents(context);
            if (components.Count > 0)
            {
                _components.Put(context, components);
            }
        }

        private List<Component> AssembleChildComponents(ParserRuleContext context)
        {
            var components = new List<Component>();
            if (context.children != null)
                foreach (var childComponents in context.children.Select(ctx => _components.Get(ctx)))
                {
                    components.AddRange(childComponents ?? new List<Component>());
                }
            return components.Distinct().ToList();
        }
    }
}
