using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ValidationWeb.Models
{
    [Serializable]
    public class RulesEngineExecutionException : ApplicationException
    {
        public RulesEngineExecutionException()
        {
        }

        protected RulesEngineExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            RuleId = info.GetString("RuleId");
            DataSourceName = info.GetString("DataSourceName");
            Sql = info.GetString("Sql");
            ExecSql = info.GetString("ExecSql");
            ChainedErrorMessages = info.GetString("ChainedErrorMessages");
        }

        public string RuleId { get; set; }
        
        public string DataSourceName { get; set; }

        public string Sql { get; set; }
        
        public string ExecSql { get; set; }
        
        public string ChainedErrorMessages { get; set; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected new virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RuleId", RuleId);
            info.AddValue("DataSourceName", DataSourceName);
            info.AddValue("Sql", Sql);
            info.AddValue("ExecSql", ExecSql);
            info.AddValue("ChainedErrorMessages", ChainedErrorMessages);

            base.GetObjectData(info, context);
        }
    }
}