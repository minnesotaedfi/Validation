using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Web;
using Moq;
using NUnit.Framework;
using Should;
using Should.Extensions;
using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Implementations;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Tests.Mocks;

namespace ValidationWeb.Tests.Services
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class EdOrgServiceTests
    {
        protected MockRepository MockRepository { get; set; }

        protected AppUserSession DefaultTestAppUserSession { get; set; }

        protected Mock<ValidationPortalDbContext> ValidationPortalDbContextMock { get; set; }

        protected Mock<IDbContextFactory<ValidationPortalDbContext>> DbContextFactoryMock { get; set; }

        protected Mock<IAppUserService> AppUserServiceMock { get; set; }
        
        protected Mock<ISchoolYearService> SchoolYearServiceMock { get; set; }

        protected Mock<ILoggingService> LoggingServiceMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);

            ValidationPortalDbContextMock = MockRepository.Create<ValidationPortalDbContext>();
            DbContextFactoryMock = MockRepository.Create<IDbContextFactory<ValidationPortalDbContext>>();
            AppUserServiceMock = MockRepository.Create<IAppUserService>();
            SchoolYearServiceMock = MockRepository.Create<ISchoolYearService>();
            LoggingServiceMock = MockRepository.Create<ILoggingService>();
        }

        [SetUp]
        public void SetUp()
        {
            DefaultTestAppUserSession = new AppUserSession
            {
                FocusedEdOrgId = 1234,
                FocusedSchoolYearId = 1,
                Id = "234",
                ExpiresUtc = DateTime.Now.AddMonths(1),
                UserIdentity = new ValidationPortalIdentity
                {
                    AuthorizedEdOrgs = new List<EdOrg>()
                },
            };

            var appUserSession = new AppUserSession
            {
                Id = "12345",
                FocusedEdOrgId = 1234,
                UserIdentity = null
            };
            
            var appUserSessions = new List<AppUserSession>(new[] { appUserSession }); 

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(appUserSessions),
                ValidationPortalDbContextMock,
                x => x.AppUserSessions,
                x => x.AppUserSessions = It.IsAny<DbSet<AppUserSession>>(),
                appUserSessions);

            var announcements = new List<Announcement>();
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            var edOrgs = new List<EdOrg>(); 
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(edOrgs),
                ValidationPortalDbContextMock,
                x => x.EdOrgs,
                x => x.EdOrgs = It.IsAny<DbSet<EdOrg>>(),
                edOrgs);

            var edOrgTypeLookups = new List<EdOrgTypeLookup>(); 
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(edOrgTypeLookups),
                ValidationPortalDbContextMock,
                x => x.EdOrgTypeLookup,
                x => x.EdOrgTypeLookup = It.IsAny<DbSet<EdOrgTypeLookup>>(),
                edOrgTypeLookups);

            var errorSeverityLookups = new List<ErrorSeverityLookup>(); 
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(errorSeverityLookups),
                ValidationPortalDbContextMock,
                x => x.ErrorSeverityLookup,
                x => x.ErrorSeverityLookup = It.IsAny<DbSet<ErrorSeverityLookup>>(),
                errorSeverityLookups);

            var recordsRequests = new List<RecordsRequest>(); 
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(recordsRequests),
                ValidationPortalDbContextMock,
                x => x.RecordsRequests,
                x => x.RecordsRequests = It.IsAny<DbSet<RecordsRequest>>(),
                recordsRequests);

            var schoolYears = new List<SchoolYear>(); 
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(schoolYears),
                ValidationPortalDbContextMock,
                x => x.SchoolYears,
                x => x.SchoolYears = It.IsAny<DbSet<SchoolYear>>(),
                schoolYears);

            var submissionCycles = new List<SubmissionCycle>(); 
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(submissionCycles),
                ValidationPortalDbContextMock,
                x => x.SubmissionCycles,
                x => x.SubmissionCycles = It.IsAny<DbSet<SubmissionCycle>>(),
                submissionCycles);

            var validationErrorSummaries = new List<ValidationErrorSummary>(); 
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(validationErrorSummaries),
                ValidationPortalDbContextMock,
                x => x.ValidationErrorSummaries,
                x => x.ValidationErrorSummaries = It.IsAny<DbSet<ValidationErrorSummary>>(),
                validationErrorSummaries);

            var validationReportDetails = new List<ValidationReportDetails>(); 
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(validationReportDetails),
                ValidationPortalDbContextMock,
                x => x.ValidationReportDetails,
                x => x.ValidationReportDetails = It.IsAny<DbSet<ValidationReportDetails>>(),
                validationReportDetails);

            var validationReportSummaries = new List<ValidationReportSummary>();
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(validationReportSummaries),
                ValidationPortalDbContextMock,
                x => x.ValidationReportSummaries,
                x => x.ValidationReportSummaries = It.IsAny<DbSet<ValidationReportSummary>>(),
                validationReportSummaries);

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            ValidationPortalDbContextMock.Reset();
            DbContextFactoryMock.Reset();
            AppUserServiceMock.Reset();
            SchoolYearServiceMock.Reset();
            LoggingServiceMock.Reset();
        }
        
        [Test]
        public void GetAuthorizedEdOrgs_Should_ReturnUserSessionEdOrgs()
        {
            var authorizedEdOrgs = new List<EdOrg>
            {
                new EdOrg { Id = 12345 },
                new EdOrg { Id = 23456 }
            };

            foreach (var edorg in authorizedEdOrgs)
            {
                DefaultTestAppUserSession.UserIdentity.AuthorizedEdOrgs.Add(edorg);
            }

            AppUserServiceMock.Setup(x => x.GetSession()).Returns(DefaultTestAppUserSession);

            var edOrgService = new EdOrgService(
                DbContextFactoryMock.Object,
                AppUserServiceMock.Object,
                SchoolYearServiceMock.Object, 
                LoggingServiceMock.Object);

            var result = edOrgService.GetAuthorizedEdOrgs();

            result.ShouldHaveSameItems(authorizedEdOrgs);
        }

        [Test]
        public void GetAllEdOrgs_Should_ReturnAllEdOrgs()
        {
            var allEdOrgs = new List<EdOrg>
            {
                new EdOrg { Id = 12345 },
                new EdOrg { Id = 23456 },
                new EdOrg { Id = 34567 }
            };

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(allEdOrgs),
                ValidationPortalDbContextMock,
                x => x.EdOrgs,
                x => x.EdOrgs = It.IsAny<DbSet<EdOrg>>(),
                allEdOrgs);


            var edOrgService = new EdOrgService(
                DbContextFactoryMock.Object,
                AppUserServiceMock.Object,
                SchoolYearServiceMock.Object, 
                LoggingServiceMock.Object);

            var result = edOrgService.GetAllEdOrgs();

            result.ShouldHaveSameItems(allEdOrgs);
        }

        [Test]
        public void GetEdOrgById_Should_ReturnSpecifiedEdOrgIfPresent()
        {
            const int testEdOrgId = 12345;
            const int testSchoolYearId = 111;
            const string testSchoolName = "12345 Elementary";

            var allEdOrgs = new List<EdOrg>
            {
                new EdOrg
                {
                    Id = testEdOrgId, 
                    SchoolYearId = testSchoolYearId, 
                    OrganizationName = testSchoolName, 
                    OrganizationShortName = testSchoolName
                },
                new EdOrg { Id = 23456 },
                new EdOrg { Id = 34567 }
            };

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(allEdOrgs),
                ValidationPortalDbContextMock,
                x => x.EdOrgs,
                x => x.EdOrgs = It.IsAny<DbSet<EdOrg>>(),
                allEdOrgs);

            var testSchoolYear = new SchoolYear
            {
                Id = testSchoolYearId,
                StartYear = "1776",
                EndYear = "1777",
                Enabled = true
            };

            var schoolYears = new List<SchoolYear>(new[] {testSchoolYear});

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(schoolYears),
                ValidationPortalDbContextMock,
                x => x.SchoolYears,
                x => x.SchoolYears = It.IsAny<DbSet<SchoolYear>>(),
                schoolYears);

            var edOrgService = new EdOrgService(
                DbContextFactoryMock.Object,
                AppUserServiceMock.Object,
                SchoolYearServiceMock.Object, 
                LoggingServiceMock.Object);

            SchoolYearServiceMock.Setup(x => x.GetSchoolYearById(testSchoolYearId)).Returns(testSchoolYear);

            LoggingServiceMock.Setup(x => x.LogDebugMessage($"EdOrg cache: {allEdOrgs.Count} currently in ValidationPortal database"));

            var result = edOrgService.GetEdOrgById(testEdOrgId, testSchoolYearId);

            result.ShouldNotBeNull();
        }
    }
}
