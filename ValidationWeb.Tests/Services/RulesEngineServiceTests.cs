using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using Should;
using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Implementations;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Tests.Mocks;

namespace ValidationWeb.Tests.Services
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class RulesEngineServiceTests
    {
        protected MockRepository MockRepository { get; set; }

        protected Mock<IRulesEngineConfigurationValues> EngineConfigMock { get; set; }

        protected Mock<IAppUserService> AppUserServiceMock { get; set; }

        protected Mock<IEdOrgService> EdOrgServiceMock { get; set; }

        protected Mock<ISchoolYearService> SchoolYearServiceMock { get; set; }

        protected Mock<ILoggingService> LoggingServiceMock { get; set; }

        protected Mock<ValidationPortalDbContext> ValidationPortalDbContextMock { get; set; }

        protected Mock<IDbContextFactory<ValidationPortalDbContext>> DbContextFactoryMock { get; set; }

        protected Mock<RawOdsDbContext> SchoolYearDbContextMock { get; set; }

        protected Mock<ISchoolYearDbContextFactory> SchoolYearDbContextFactoryMock { get; set; }

        protected AppUserSession DefaultTestAppUserSession { get; set; }

        protected Engine.Models.Model EngineObjectModel { get; set; }

        protected Mock<IOdsConfigurationValues> OdsConfigurationValuesMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);

            EngineConfigMock = MockRepository.Create<IRulesEngineConfigurationValues>();
            AppUserServiceMock = MockRepository.Create<IAppUserService>();
            EdOrgServiceMock = MockRepository.Create<IEdOrgService>();
            SchoolYearServiceMock = MockRepository.Create<ISchoolYearService>();
            LoggingServiceMock = MockRepository.Create<ILoggingService>();
            ValidationPortalDbContextMock = MockRepository.Create<ValidationPortalDbContext>();
            DbContextFactoryMock = MockRepository.Create<IDbContextFactory<ValidationPortalDbContext>>();

            OdsConfigurationValuesMock = MockRepository.Create<IOdsConfigurationValues>();

            SchoolYearDbContextMock = MockRepository.Create<RawOdsDbContext>(OdsConfigurationValuesMock.Object, It.IsAny<string>());

            SchoolYearDbContextFactoryMock = MockRepository.Create<ISchoolYearDbContextFactory>();
        }

        [SetUp]
        public void SetUp()
        {
            LoggingServiceMock.Setup(x => x.LogDebugMessage(It.IsAny<string>()));
            LoggingServiceMock.Setup(x => x.LogErrorMessage(It.IsAny<string>()));
            LoggingServiceMock.Setup(x => x.LogWarningMessage(It.IsAny<string>()));
            LoggingServiceMock.Setup(x => x.LogInfoMessage(It.IsAny<string>()));

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

            var validationRulesViews = new List<ValidationRulesView>();
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(validationRulesViews),
                ValidationPortalDbContextMock,
                x => x.ValidationRulesViews,
                x => x.ValidationRulesViews = It.IsAny<DbSet<ValidationRulesView>>(),
                validationRulesViews);

            var validationRulesFields = new List<ValidationRulesField>();
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(validationRulesFields),
                ValidationPortalDbContextMock,
                x => x.ValidationRulesFields,
                x => x.ValidationRulesFields = It.IsAny<DbSet<ValidationRulesField>>(),
                validationRulesFields);

            DbContextFactoryMock.As<IDisposable>().Setup(x => x.Dispose());

            DbContextFactoryMock
                .Setup(x => x.Create())
                .Returns(ValidationPortalDbContextMock.Object);

            OdsConfigurationValuesMock
                .Setup(x => x.GetRawOdsConnectionString(It.IsAny<string>()))
                .Returns<string>(x => $"Test Connection string: EdFi_Ods_{x};");

            var ruleValidations = new List<RuleValidation>();

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(ruleValidations),
                SchoolYearDbContextMock,
                x => x.RuleValidations,
                x => x.RuleValidations = It.IsAny<DbSet<RuleValidation>>(),
                ruleValidations);

            var ruleValidationDetails = new List<RuleValidationDetail>();

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(ruleValidationDetails),
                SchoolYearDbContextMock,
                x => x.RuleValidationDetails,
                x => x.RuleValidationDetails = It.IsAny<DbSet<RuleValidationDetail>>(),
                ruleValidationDetails);

            var ruleValidationRuleComponents = new List<RuleValidationRuleComponent>();

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(ruleValidationRuleComponents),
                SchoolYearDbContextMock,
                x => x.RuleValidationRuleComponents,
                x => x.RuleValidationRuleComponents = It.IsAny<DbSet<RuleValidationRuleComponent>>(),
                ruleValidationRuleComponents);

            SchoolYearDbContextMock.As<IDisposable>().Setup(x => x.Dispose());

            SchoolYearDbContextFactoryMock
                .Setup(x => x.CreateWithParameter(It.IsAny<string>()))
                .Returns(SchoolYearDbContextMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            EngineConfigMock.Reset();
            AppUserServiceMock.Reset();
            EdOrgServiceMock.Reset();
            SchoolYearServiceMock.Reset();
            LoggingServiceMock.Reset();
            ValidationPortalDbContextMock.Reset();
            OdsConfigurationValuesMock.Reset();
            SchoolYearDbContextFactoryMock.Reset();
            DbContextFactoryMock.Reset();
            SchoolYearDbContextMock.Reset();
        }

        [Test]
        public void SetupValidationRun_Should_ThrowForNullSubmissionCycle()
        {
            var rulesEngineService = new RulesEngineService(
                AppUserServiceMock.Object,
                EdOrgServiceMock.Object,
                SchoolYearServiceMock.Object,
                EngineConfigMock.Object,
                LoggingServiceMock.Object,
                DbContextFactoryMock.Object,
                SchoolYearDbContextFactoryMock.Object,
                EngineObjectModel);

            Assert.Throws<ArgumentException>(() => rulesEngineService.SetupValidationRun(null, null));
        }

        [Test]
        public void SetupValidationRun_Should_ThrowForNullSchoolYearId()
        {
            var rulesEngineService = new RulesEngineService(
                AppUserServiceMock.Object,
                EdOrgServiceMock.Object,
                SchoolYearServiceMock.Object,
                EngineConfigMock.Object,
                LoggingServiceMock.Object,
                DbContextFactoryMock.Object,
                SchoolYearDbContextFactoryMock.Object,
                EngineObjectModel);

            var submissionCycle = new SubmissionCycle { SchoolYearId = null };
            Assert.Throws<ArgumentException>(() => rulesEngineService.SetupValidationRun(submissionCycle, null));
        }

        [Test]
        public void SetupValidationRun_Should_ReturnValidationReportSummary()
        {
            var rulesEngineService = new RulesEngineService(
                AppUserServiceMock.Object,
                EdOrgServiceMock.Object,
                SchoolYearServiceMock.Object,
                EngineConfigMock.Object,
                LoggingServiceMock.Object,
                DbContextFactoryMock.Object,
                SchoolYearDbContextFactoryMock.Object,
                EngineObjectModel);

            var schoolYear = new SchoolYear
            {
                Id = 1,
                Enabled = true,
                ErrorThreshold = null,
                StartYear = "2019",
                EndYear = "2020"
            };

            var submissionCycle = new SubmissionCycle { SchoolYearId = schoolYear.Id, CollectionId = "collection" };
            SchoolYearServiceMock.Setup(x => x.GetSchoolYearById(schoolYear.Id)).Returns(schoolYear);

            AppUserServiceMock.Setup(x => x.GetSession()).Returns(DefaultTestAppUserSession);
            AppUserServiceMock.Setup(x => x.GetUser()).Returns(DefaultTestAppUserSession.UserIdentity);
            
            var result = rulesEngineService.SetupValidationRun(submissionCycle, submissionCycle.CollectionId);

            result.ShouldNotBeNull();
            result.SchoolYearId.ShouldEqual(schoolYear.Id);
            result.Collection.ShouldEqual(submissionCycle.CollectionId);

            SchoolYearDbContextMock.Verify(
                x => x.RuleValidations.Add(
                    It.Is<RuleValidation>(y => y.CollectionId == submissionCycle.CollectionId)));
        }
    }
}
