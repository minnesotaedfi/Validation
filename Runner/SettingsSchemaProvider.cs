using Engine.Language;

namespace Runner
{
    public class SettingsSchemaProvider: ISchemaProvider
    {
        public string Value => Properties.Settings.Default.Schema;
    }
}
