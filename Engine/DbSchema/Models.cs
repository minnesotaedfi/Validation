namespace Engine.DbSchema
{
    public class Column
    {
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
    }

    public class Parameter
    {
        public string Schema { get; set; }
        public string FunctionName { get; set; }
        public string ParameterName { get; set; }
        public int OrdinalPosition { get; set; }
        public string ParameterMode { get; set; }
        public string IsResult { get; set; }
        public string DataType { get; set; }
    }
}
