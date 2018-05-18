namespace Engine.Language
{
    public interface ISchemaProvider
    {
        string Value { get; }
    }
    
    public class SchemaProvider : ISchemaProvider
    {
        public string Value => "dbo";
    }
}
