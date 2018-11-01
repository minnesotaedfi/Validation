using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class RulesEngineExecutionException : ApplicationException
    {
        public string RuleId { get; set; }
        public string DataSourceName { get; set; }
        public string Sql { get; set; }
        public string ExecSql { get; set; }
        public string ChainedErrorMessages { get; set; }
    }
}