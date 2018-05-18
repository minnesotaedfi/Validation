using System.Collections.Generic;
using System.Linq;
using Engine.Utility;
using static System.Configuration.ConfigurationManager;

namespace Runner
{
    public class SettingsConstantValueProvider : IConstantValueProvider
    {
        public IDictionary<string, string> Values => AppSettings.AllKeys.Where(x => x.StartsWith("constant."))
            .ToDictionary(key => key.Replace("constant.", string.Empty), key => AppSettings[key]);
    }
}
