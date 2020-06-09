using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Validation.DataModels;

using ValidationWeb.Database;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class ProgramAreaService : IProgramAreaService
    {
        public ProgramAreaService(IDbContextFactory<ValidationPortalDbContext> validationPortalDataContextFactory)
        {
            ValidationPortalDataContextFactory = validationPortalDataContextFactory;
        }

        private IDbContextFactory<ValidationPortalDbContext> ValidationPortalDataContextFactory { get; }

        public IList<ProgramAreaLookup> GetProgramAreas()
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalDataContext.ProgramAreaLookup.ToList();
            }
        }

        public ProgramAreaLookup GetProgramAreaById(int programAreaId)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalDataContext.ProgramAreaLookup.FirstOrDefault(x => x.Id == programAreaId);
            }
        }
    }
}
