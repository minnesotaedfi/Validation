using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EngineTest.Db
{
    public class EngineDbTestContext : DbContext
    {
        static EngineDbTestContext()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EngineDbTestContext>());
        }

        public EngineDbTestContext() : base()
        {
        }

        public EngineDbTestContext(string connectionString) : base(connectionString)
        {
        }

        public DbSet<Component1> Component1S { get; set; }
        public DbSet<Component2> Component2S { get; set; }
    }

    public class Component1
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Component1Id { get; set; }
        public long Id { get; set; }
        public string StringCharacteristic1 { get; set; }
        public DateTime? DateCharacteristic1 { get; set; }
        public decimal? NumberCharacteristic1 { get; set; }
        public bool? BoolCharacteristic1 { get; set; }
        public string StringCharacteristic2 { get; set; }
        public DateTime? DateCharacteristic2 { get; set; }
        public decimal? NumberCharacteristic2 { get; set; }
        public bool? BoolCharacteristic2 { get; set; }
    }

    public class Component2
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Component2Id { get; set; }
        public long Id { get; set; }
        public string StringCharacteristic1 { get; set; }
        public DateTime? DateCharacteristic1 { get; set; }
        public decimal? NumberCharacteristic1 { get; set; }
        public bool? BoolCharacteristic1 { get; set; }
        public string StringCharacteristic2 { get; set; }
        public DateTime? DateCharacteristic2 { get; set; }
        public decimal? NumberCharacteristic2 { get; set; }
        public bool? BoolCharacteristic2 { get; set; }
    }
}
