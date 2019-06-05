using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Engine.DbSchema
{
    public class Loader : IDisposable
    {

        private readonly Lazy<IReadOnlyList<Column>> _tables;
        private readonly Lazy<IReadOnlyList<Parameter>> _functions;

        public Loader(DbConnection connection)
        {
            _context = new DbContext(connection, false);
            _tables = new Lazy<IReadOnlyList<Column>>(GetTables);
            _functions = new Lazy<IReadOnlyList<Parameter>>(GetFunctions);
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        protected DbContext _context { get; private set; }
        
        public IReadOnlyList<Column> Tables => _tables.Value;

        public IReadOnlyList<Parameter> Functions => _functions.Value;

        private IReadOnlyList<Column> GetTables()
        {
            return GetTablesAsync().Result;
        }

        public async Task<List<Column>> GetTablesAsync()
        {
            const string sql =
                "SELECT TABLE_SCHEMA [Schema], TABLE_NAME [TableName], COLUMN_NAME [ColumnName], DATA_TYPE [DataType] " +
                "FROM INFORMATION_SCHEMA.COLUMNS";
            var query = _context.Database.SqlQuery<Column>(sql);
            return await query.ToListAsync();
        }

        private IReadOnlyList<Parameter> GetFunctions()
        {
            return GetFunctionsAsync().Result;
        }

        public async Task<List<Parameter>> GetFunctionsAsync()
        {
            const string sql =
                "SELECT SPECIFIC_SCHEMA [Schema], SPECIFIC_NAME [FunctionName], ORDINAL_POSITION [OrdinalPosition], " +
                "PARAMETER_NAME [ParameterName], PARAMETER_MODE [ParameterMode], IS_RESULT [IsResult], DATA_TYPE [DataType] " +
                "FROM INFORMATION_SCHEMA.PARAMETERS";
            var query = _context.Database.SqlQuery<Parameter>(sql);
            return await query.ToListAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _context?.Dispose();
        }
    }
}
