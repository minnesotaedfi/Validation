using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

using Validation.DataModels;

using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;

namespace ValidationWeb.Services.Implementations
{
    public class SubmissionCycleService : ISubmissionCycleService
    {
        public SubmissionCycleService(
            IDbContextFactory<ValidationPortalDbContext> validationPortalDataContextFactory,
            ISchoolYearService schoolYearService,
            ILoggingService loggingService)
        {
            ValidationPortalDataContextFactory = validationPortalDataContextFactory;
            SchoolYearService = schoolYearService;
            LoggingService = loggingService;
        }

        protected IDbContextFactory<ValidationPortalDbContext> ValidationPortalDataContextFactory { get; set; }

        protected ISchoolYearService SchoolYearService { get; set; }

        protected ILoggingService LoggingService { get; set; }

        public IList<SubmissionCycle> GetSubmissionCycles(ProgramAreaLookup programArea = null)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                var submissionCycles = validationPortalDataContext.SubmissionCycles.ToList();

                if(programArea != null)
                {
                    submissionCycles = submissionCycles.Where(x => x.ProgramAreaId == programArea.Id).ToList();
                }

                foreach (var submissionCycle in submissionCycles)
                {
                    var schoolYear = validationPortalDataContext.SchoolYears.FirstOrDefault(x => x.Id == submissionCycle.SchoolYearId);
                    if (schoolYear != null)
                    {
                        submissionCycle.SchoolYearDisplay = schoolYear.ToString();
                    }
                }

                return submissionCycles.ToList();
            }
        }

        public IEnumerable<SubmissionCycle> GetSubmissionCyclesOpenToday(ProgramAreaLookup programArea = null)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                var submissionCycles = validationPortalDataContext.SubmissionCycles.ToList();

                var submissionCyclesOpenToday = submissionCycles
                    .Where(s => 
                        s.StartDate.Date <= DateTime.Now.Date &&
                        s.EndDate.Date >= DateTime.Now.Date)
                    .Where(x => programArea == null || x.ProgramAreaId == programArea.Id)
                    .ToList();

                foreach (var submissionCycle in submissionCyclesOpenToday)
                {
                    var schoolYear =
                        validationPortalDataContext.SchoolYears.FirstOrDefault(
                            x => x.Id == submissionCycle.SchoolYearId);

                    if (schoolYear != null)
                    {
                        submissionCycle.SchoolYearDisplay = schoolYear.ToString();
                    }
                }

                return submissionCyclesOpenToday;
            }
        }

        public SubmissionCycle GetSubmissionCycle(int id)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalDataContext.SubmissionCycles
                    .FirstOrDefault(submissionCycle => submissionCycle.Id == id);
            }
        }

        public bool AddSubmissionCycle(SubmissionCycle submissionCycle)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                validationPortalDataContext.SubmissionCycles.Add(submissionCycle);
                validationPortalDataContext.SaveChanges();

                return true; // why have a return type at all? todo
            }
        }

        public bool AddSubmissionCycle(string collectionId, DateTime startDate, DateTime endDate)
        {
            if (collectionId == null)
            {
                // todo: better null/invalid argument handling 
                return false;
            }

            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                var newSubmissionCycle = new SubmissionCycle(collectionId, startDate, endDate);
                validationPortalDataContext.SubmissionCycles.Add(newSubmissionCycle);
                validationPortalDataContext.SaveChanges();

                return true;
            }
        }

        public void SaveSubmissionCycle(SubmissionCycle submissionCycle)
        {
            try
            {
                using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
                {
                    if (submissionCycle == null)
                    {
                        throw new Exception("Attempted to save a null SubmissionCycle.");
                    }

                    if (submissionCycle.Id == 0)
                    {
                        validationPortalDataContext.SubmissionCycles.Add(submissionCycle);
                        validationPortalDataContext.SaveChanges();
                    }
                    else
                    {
                        var existingSubmissionCycle = validationPortalDataContext.SubmissionCycles
                            .FirstOrDefault(s => s.Id == submissionCycle.Id);

                        if (existingSubmissionCycle == null)
                        {
                            throw new Exception($"SubmissionCycle with id {submissionCycle.Id} does not exist.");
                        }

                        existingSubmissionCycle.CollectionId = submissionCycle.CollectionId;
                        existingSubmissionCycle.StartDate = submissionCycle.StartDate;
                        existingSubmissionCycle.EndDate = submissionCycle.EndDate;
                        existingSubmissionCycle.SchoolYearId = submissionCycle.SchoolYearId;
                        validationPortalDataContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                string strMessage = string.Empty;
                if (submissionCycle != null)
                {
                    strMessage = $" with id = { submissionCycle.Id }, StartDate = { submissionCycle.StartDate }, EndDate = { submissionCycle.EndDate}, SchoolYearID = { submissionCycle.SchoolYearId }";
                }

                LoggingService.LogErrorMessage($"An error occurred while saving announcement {strMessage}: {ex.ChainInnerExceptionMessages()}");
                throw new Exception("An error occurred while saving announcement.");
            }
        }

        public IList<SubmissionCycle> GetSubmissionCyclesByCollectionId(string collectionId)
        {
            if (collectionId == null)
            {
                return null;
            }

            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalDataContext.SubmissionCycles
                    .Where(submissionCycle => submissionCycle.CollectionId == collectionId)
                    .ToList();
            }
        }

        public List<SelectListItem> GetSchoolYearsSelectList(SubmissionCycle submissionCycle = null)
        {
            var schoolYearsEnumerable = SchoolYearService.GetSubmittableSchoolYearsDictionary().OrderByDescending(x => x.Value);

            List<SelectListItem> schoolYears = schoolYearsEnumerable.Select(kvPair => new SelectListItem
            {
                Value = kvPair.Key.ToString(),
                Text = kvPair.Value,
                Selected = (submissionCycle != null) && (kvPair.Key == submissionCycle.SchoolYearId)
            }).ToList();

            if (submissionCycle != null)
            {
                schoolYears[0].Selected = true;
            }
            return schoolYears;
        }

        public SubmissionCycle SchoolYearCollectionAlreadyExists(SubmissionCycle submissionCycle)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                var duplicateCycle = validationPortalDataContext.SubmissionCycles
                    .FirstOrDefault(x => x.SchoolYearId == submissionCycle.SchoolYearId && x.CollectionId == submissionCycle.CollectionId);

                return duplicateCycle;
            }
        }
        public void DeleteSubmissionCycle(int id)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                var submissionCycle = validationPortalDataContext.SubmissionCycles.FirstOrDefault(a => a.Id == id);

                if (submissionCycle == null)
                {
                    throw new Exception(
                        $"Could not delete a collection cycle because collection cycle with ID {id} was not found");
                }

                validationPortalDataContext.SubmissionCycles.Remove(submissionCycle);
                validationPortalDataContext.SaveChanges();
            }
        }
    }
}