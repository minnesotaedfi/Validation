using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet;

namespace Engine.Templates
{
    public class TemplateEngine
    {
        private readonly IHandlebars _handlebars;

        public TemplateEngine(object handlebars = null)
        {
            _handlebars = handlebars as IHandlebars ?? Handlebars.Create();
            Initialize();
        }

        private void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var templateNames = assembly.GetManifestResourceNames().Where(x => x.EndsWith(".hbs"));
            foreach (var resourceName in templateNames)
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName) ?? new MemoryStream())
                using (var reader = new StreamReader(stream))
                {
                    var partialTemplate = Handlebars.Compile(reader);
                    _handlebars.RegisterTemplate(ShortTemplateName(resourceName), partialTemplate);
                }
            }
        }

        private static string ShortTemplateName(string fullTemplateName)
        {
            // naive approach takes the next to last portion of the full name
            char[] delimiters = { '.' };
            var segments = fullTemplateName.Split(delimiters);
            return segments[segments.Count() - 2];
        }

        public string Generate(string templateName, object data)
        {
            if (_handlebars.Configuration.RegisteredTemplates.ContainsKey(templateName))
            {
                var template = _handlebars.Configuration.RegisteredTemplates[templateName];
                var writer = new StringWriter();
                template(writer, data);
                return writer.ToString();
            }
            else throw new Exception($"Handlebars template '{templateName}' was not found");
        }
    }
}
