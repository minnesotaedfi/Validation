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

        public IList<ProgramArea> GetProgramAreas()
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalDataContext.ProgramAreas.ToList();
            }
        }

        public ProgramArea GetProgramAreaById(int programAreaId)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalDataContext.ProgramAreas.FirstOrDefault(x => x.Id == programAreaId);
            }
        }
    }
}
