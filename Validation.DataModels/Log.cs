using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValidationWeb.Models
{
    [Table("dbo.Log")]
    public class Log
    {        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime? Date { get; set; }

        public string Thread { get; set; }
        
        public string Level { get; set; }

        public string Logger { get; set; }

        public string Year { get; set; }

        public string District { get; set; }

        public string Method { get; set; }

        public string Url { get; set; }

        public string ResponseCode { get; set; }

        public string ResponsePhrase { get; set; }

        public string ResponseBody { get; set; }

        public string Exception { get; set; }
    }
}