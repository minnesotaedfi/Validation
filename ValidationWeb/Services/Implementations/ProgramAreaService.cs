using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Validation.DataModels;

using ValidationWeb.Database;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;

namespace ValidationWeb.Services.Implementations
{
    public class ProgramAreaService : IProgramAreaService
    {
        public ProgramAreaService(
            IDbContextFactory<ValidationPortalDbContext> validationPortalDataContextFactory,
            ILoggingService loggingService)
        {
            ValidationPortalDataContextFactory = validationPortalDataContextFactory;
            LoggingService = loggingService;
        }

        private IDbContextFactory<ValidationPortalDbContext> ValidationPortalDataContextFactory { get; }
        private ILoggingService LoggingService { get; }

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

        public void AddProgramArea(ProgramArea programArea)
        {
            if (programArea == null)
            {
                throw new Exception("Attempted to add a null ProgramArea.");
            }

            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                validationPortalDataContext.ProgramAreas.Add(programArea);
                validationPortalDataContext.SaveChanges();
            }
        }

        public void SaveProgramArea(ProgramArea programArea)
        {
            if (programArea == null)
            {
                throw new Exception("Attempted to save a null ProgramArea.");
            }

            try
            {
                using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
                {
                    if (programArea.Id == 0)
                    {
                        validationPortalDataContext.ProgramAreas.Add(programArea);
                        validationPortalDataContext.SaveChanges();
                    }
                    else
                    {
                        var existingProgramArea = validationPortalDataContext.ProgramAreas
                            .FirstOrDefault(x => x.Id == programArea.Id);

                        if (existingProgramArea == null)
                        {
                            throw new Exception($"ProgramArea with id {programArea.Id} does not exist.");
                        }

                        existingProgramArea.Description = programArea.Description;
                        validationPortalDataContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                var strMessage = $" with id = { programArea.Id }, Description { programArea.Description }";
                LoggingService.LogErrorMessage($"An error occurred while saving program area {strMessage}: {ex.ChainInnerExceptionMessages()}");
                throw new Exception("An error occurred while saving program area.");
            }
        }

        public void DeleteProgramArea(int id)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                var programArea = validationPortalDataContext.ProgramAreas.FirstOrDefault(x => x.Id == id);

                if (programArea == null)
                {
                    throw new Exception(
                        $"Could not delete program area with ID {id} - not found");
                }

                // must remove all associations first!
                var submissionCycles = validationPortalDataContext.SubmissionCycles.Where(x => x.ProgramAreaId == programArea.Id);
                foreach (var submissionCycle in submissionCycles)
                {
                    submissionCycle.ProgramArea = null;
                    submissionCycle.ProgramAreaId = null; 
                }

                var dynamicReports = validationPortalDataContext.DynamicReportDefinitions.Where(x => x.ProgramAreaId == programArea.Id);
                foreach (var dynamicReport in dynamicReports)
                {
                    dynamicReport.ProgramArea = null;
                    dynamicReport.ProgramAreaId = null;
                }

                var announcements = validationPortalDataContext.Announcements.Where(x => x.ProgramAreaId == programArea.Id);
                foreach (var announcement in announcements)
                {
                    announcement.ProgramArea = null;
                    announcement.ProgramAreaId = null;
                }

                validationPortalDataContext.ProgramAreas.Remove(programArea);
                validationPortalDataContext.SaveChanges();
            }
        }
    }
}
