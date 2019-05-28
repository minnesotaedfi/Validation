using System.Data.Entity;
using ValidationWeb.Models;

namespace ValidationWeb.Database
{
    public class EdFiLogDbContext : DbContext
    {
        public virtual DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            System.Data.Entity.Database.SetInitializer<EdFiLogDbContext>(null);
        }
    }
}

