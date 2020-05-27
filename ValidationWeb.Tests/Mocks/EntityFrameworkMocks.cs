using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Moq;

using Validation.DataModels;

using ValidationWeb.Database;
using ValidationWeb.Models;

namespace ValidationWeb.Tests.Mocks
{
    [ExcludeFromCodeCoverage]
    public class EntityFrameworkMocks
    {
        static EntityFrameworkMocks()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);
        }

        protected static MockRepository MockRepository { get; set; }

        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) 
            where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = MockRepository.Create<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(sourceList.Add).Returns<T>(x => x);
            dbSet.Setup(x => x.Remove(It.IsAny<T>())).Callback<T>(x => sourceList.Remove(x)).Returns<T>(x => x);
            return dbSet;
        }

        public static void SetupMockDbSet<T, TU>(
            Mock<DbSet<TU>> queryableMockDbSet,
            Mock<T> dbContextMock,
            Expression<Func<T, DbSet<TU>>> setupExpression,
            Action<T> setterExpression,
            List<TU> source)
            where T : DbContext
            where TU : class
        {
            dbContextMock.Setup(x => x.Set<TU>()).Returns(queryableMockDbSet.Object);
            dbContextMock.Setup(setupExpression).Returns(() => queryableMockDbSet.Object);
            dbContextMock.Setup(x => x.SaveChanges()).Returns(1);
            dbContextMock.SetupSet(setterExpression);
        }

        public static void SetupValidationPortalDbContext(Mock<ValidationPortalDbContext> validationPortalDbContextMock)
        {
            var appUserSession = new AppUserSession
            {
                Id = "12345",
                FocusedEdOrgId = 1234,
                UserIdentity = null
            };
            
            var appUserSessions = new List<AppUserSession>(new[] { appUserSession }); 

            SetupMockDbSet(
                GetQueryableMockDbSet(appUserSessions),
                validationPortalDbContextMock,
                x => x.AppUserSessions,
                x => x.AppUserSessions = It.IsAny<DbSet<AppUserSession>>(),
                appUserSessions);

            var announcements = new List<Announcement>();
            SetupMockDbSet(
                GetQueryableMockDbSet(announcements),
                validationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            var edOrgs = new List<EdOrg>(); 
            SetupMockDbSet(
                GetQueryableMockDbSet(edOrgs),
                validationPortalDbContextMock,
                x => x.EdOrgs,
                x => x.EdOrgs = It.IsAny<DbSet<EdOrg>>(),
                edOrgs);

            var edOrgTypeLookups = new List<EdOrgTypeLookup>(); 
            SetupMockDbSet(
                GetQueryableMockDbSet(edOrgTypeLookups),
                validationPortalDbContextMock,
                x => x.EdOrgTypeLookup,
                x => x.EdOrgTypeLookup = It.IsAny<DbSet<EdOrgTypeLookup>>(),
                edOrgTypeLookups);

            var errorSeverityLookups = new List<ErrorSeverityLookup>(); 
            SetupMockDbSet(
                GetQueryableMockDbSet(errorSeverityLookups),
                validationPortalDbContextMock,
                x => x.ErrorSeverityLookup,
                x => x.ErrorSeverityLookup = It.IsAny<DbSet<ErrorSeverityLookup>>(),
                errorSeverityLookups);

            var recordsRequests = new List<RecordsRequest>(); 
            SetupMockDbSet(
                GetQueryableMockDbSet(recordsRequests),
                validationPortalDbContextMock,
                x => x.RecordsRequests,
                x => x.RecordsRequests = It.IsAny<DbSet<RecordsRequest>>(),
                recordsRequests);

            var schoolYears = new List<SchoolYear>(); 
            SetupMockDbSet(
                GetQueryableMockDbSet(schoolYears),
                validationPortalDbContextMock,
                x => x.SchoolYears,
                x => x.SchoolYears = It.IsAny<DbSet<SchoolYear>>(),
                schoolYears);

            var submissionCycles = new List<SubmissionCycle>(); 
            SetupMockDbSet(
                GetQueryableMockDbSet(submissionCycles),
                validationPortalDbContextMock,
                x => x.SubmissionCycles,
                x => x.SubmissionCycles = It.IsAny<DbSet<SubmissionCycle>>(),
                submissionCycles);

            var validationErrorSummaries = new List<ValidationErrorSummary>(); 
            SetupMockDbSet(
                GetQueryableMockDbSet(validationErrorSummaries),
                validationPortalDbContextMock,
                x => x.ValidationErrorSummaries,
                x => x.ValidationErrorSummaries = It.IsAny<DbSet<ValidationErrorSummary>>(),
                validationErrorSummaries);

            var validationReportDetails = new List<ValidationReportDetails>(); 
            SetupMockDbSet(
                GetQueryableMockDbSet(validationReportDetails),
                validationPortalDbContextMock,
                x => x.ValidationReportDetails,
                x => x.ValidationReportDetails = It.IsAny<DbSet<ValidationReportDetails>>(),
                validationReportDetails);

            var validationReportSummaries = new List<ValidationReportSummary>();
            SetupMockDbSet(
                GetQueryableMockDbSet(validationReportSummaries),
                validationPortalDbContextMock,
                x => x.ValidationReportSummaries,
                x => x.ValidationReportSummaries = It.IsAny<DbSet<ValidationReportSummary>>(),
                validationReportSummaries);
                
            var validationRulesViews = new List<ValidationRulesView>();
            SetupMockDbSet(
                GetQueryableMockDbSet(validationRulesViews),
                validationPortalDbContextMock,
                x => x.ValidationRulesViews,
                x => x.ValidationRulesViews = It.IsAny<DbSet<ValidationRulesView>>(),
                validationRulesViews);

            var validationRulesFields = new List<ValidationRulesField>();
            SetupMockDbSet(
                GetQueryableMockDbSet(validationRulesFields),
                validationPortalDbContextMock,
                x => x.ValidationRulesFields,
                x => x.ValidationRulesFields = It.IsAny<DbSet<ValidationRulesField>>(),
                validationRulesFields);

            var dynamicReportDefinitions = new List<DynamicReportDefinition>(); 
            SetupMockDbSet(
                GetQueryableMockDbSet(dynamicReportDefinitions),
                validationPortalDbContextMock,
                x => x.DynamicReportDefinitions,
                x => x.DynamicReportDefinitions = It.IsAny<DbSet<DynamicReportDefinition>>(),
                dynamicReportDefinitions);

            var dynamicReportFields = new List<DynamicReportField>(); 
            SetupMockDbSet(
                GetQueryableMockDbSet(dynamicReportFields),
                validationPortalDbContextMock,
                x => x.DynamicReportFields,
                x => x.DynamicReportFields = It.IsAny<DbSet<DynamicReportField>>(),
                dynamicReportFields);

            var programAreaLookups = new List<ProgramAreaLookup>();
            SetupMockDbSet(
                GetQueryableMockDbSet(programAreaLookups),
                validationPortalDbContextMock,
                x => x.ProgramAreaLookup,
                x => x.ProgramAreaLookup = It.IsAny<DbSet<ProgramAreaLookup>>(),
                programAreaLookups);

            validationPortalDbContextMock.As<IDisposable>().Setup(x => x.Dispose());
        }
    }
}