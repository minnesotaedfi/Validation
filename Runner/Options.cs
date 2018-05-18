using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Engine.Language;
using Engine.Utility;

namespace Runner
{
    public class Options : ISchemaProvider, IConstantValueProvider
    {
        public Options()
        {
            ConnectionString = "Default";
            RulesPath = Directory.GetCurrentDirectory();
            Schema = string.IsNullOrEmpty(Properties.Settings.Default.Schema) ? "dbo" : Properties.Settings.Default.Schema;
        }

        [Option('c', "collection", HelpText = "the collection, ruleset or rule id to run", SetName = "cmd")]
        public string Collection { get; set; }

        [Option('d', "database", DefaultValue = "Default", HelpText = "database connection string name from .config file", SetName = "cmd")]
        public string ConnectionString { get; set; }

        [Option('r', "rules", HelpText = "directory containing rules", SetName = "cmd")]
        public string RulesPath { get; set; }

        [Option('p', "port", DefaultValue = 8080, HelpText = "port for http web server", SetName = "web")]
        public int Port { get; set; }

        [Option('s', "schema", HelpText = "the schema to use as a prefix for database tables, views, and functions", SetName = "cmd")]
        public string Schema { get; set; }

        [Option('t', "test", DefaultValue = false, HelpText = "run in test mode, do not store the results", SetName = "cmd")]
        public bool Test { get; set; } = false;

        [Option('v', "variables", HelpText = "run time values for constants defined in rules ( name1=value1,name2=value2 )", SetName = "cmd")]
        public string Variables { get; set; }

        public bool Web => string.IsNullOrEmpty(Collection);

        string ISchemaProvider.Value => Schema;

        public IDictionary<string, string> Values =>
            string.IsNullOrEmpty(Variables)
                ? new Dictionary<string, string>()
                : Variables
                    .Split(',')
                    .Where(x => x.Contains("="))
                    .Select(nvp => nvp.Split('='))
                    .ToDictionary(s => s[0], s => s[1]);
    }
}
