namespace ValidationWeb
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;

    using ValidationWeb.Services;

    public class RawOdsDbContext : DbContext
    {
        static RawOdsDbContext()
        {
            // Fixes a known bug in which EntityFramework.SqlServer.dll is not copied into consumer even when CopyLocal is True.
            var includeSqlServerDLLInConsumer = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
        
        public RawOdsDbContext()
        {
        }

        // Use of connection string ensures that a new database won't be created by default.
        public RawOdsDbContext(string fourDigitYear)
            : base(new OdsConfigurationValues().GetRawOdsConnectionString(fourDigitYear))
        {
        }

        public virtual IDbConnection Connection => Database.Connection;

        public virtual IDbCommand CreateCommand()
        {
            return Database.Connection.CreateCommand();
        }

        public virtual void AddOrUpdate<T>(T item)
        {
            if (typeof(T) == typeof(RuleValidation))
            {
                RuleValidations.AddOrUpdate(item as RuleValidation);
            }
            if (typeof(T) == typeof(RuleValidationDetail))
            {
                RuleValidationDetails.AddOrUpdate(item as RuleValidationDetail);
            }
            if (typeof(T) == typeof(RuleValidation))
            {
                RuleValidationRuleComponents.AddOrUpdate(item as RuleValidationRuleComponent);
            }
        }

        public virtual DbSet<RuleValidation> RuleValidations { get; set; }

        public virtual DbSet<RuleValidationDetail> RuleValidationDetails { get; set; }

        public virtual DbSet<RuleValidationRuleComponent> RuleValidationRuleComponents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RuleValidationDetail>()
               .HasRequired(dt => dt.RuleValidation)
               .WithMany(dt => dt.RuleValidationDetails)
               .HasForeignKey(dt => dt.RuleValidationId)
               .WillCascadeOnDelete(true);

            modelBuilder.Entity<RuleValidationRuleComponent>()
               .HasRequired(dt => dt.RuleValidation)
               .WithMany(dt => dt.RuleValidationRuleComponents)
               .HasForeignKey(dt => dt.RuleValidationId)
               .WillCascadeOnDelete(true);
        }
    }
}

