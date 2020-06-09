using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;

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
        private MockRepository MockRepository { get; set; }
        private AppUserSession DefaultTestAppUserSession { get; set; }
        private Mock<ValidationPortalDbContext> ValidationPortalDbContextMock { get; set; }
        private Mock<IDbContextFactory<ValidationPortalDbContext>> DbContextFactoryMock { get; set; }
        private Mock<ISchoolYearDbContextFactory> SchoolYearDbContextFactoryMock { get; set; }
        private Mock<IAppUserService> AppUserServiceMock { get; set; }
        private Mock<ISchoolYearService> SchoolYearServiceMock { get; set; }
        private Mock<ILoggingService> LoggingServiceMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);

            ValidationPortalDbContextMock = MockRepository.Create<ValidationPortalDbContext>();
            DbContextFactoryMock = MockRepository.Create<IDbContextFactory<ValidationPortalDbContext>>();
            AppUserServiceMock = MockRepository.Create<IAppUserService>();
            SchoolYearServiceMock = MockRepository.Create<ISchoolYearService>();
            SchoolYearDbContextFactoryMock = MockRepository.Create<ISchoolYearDbContextFactory>();
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

            EntityFrameworkMocks.SetupValidationPortalDbContext(ValidationPortalDbContextMock);
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
                SchoolYearDbContextFactoryMock.Object,
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
                SchoolYearDbContextFactoryMock.Object,
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
                SchoolYearDbContextFactoryMock.Object,
                LoggingServiceMock.Object);

            SchoolYearServiceMock.Setup(x => x.GetSchoolYearById(testSchoolYearId)).Returns(testSchoolYear);

            LoggingServiceMock.Setup(x => x.LogDebugMessage($"GetEdOrgById: '{testEdOrgId}', year {testSchoolYearId}"));
            LoggingServiceMock.Setup(x => x.LogDebugMessage($"EdOrg cache: {allEdOrgs.Count} currently in ValidationPortal database"));

            var result = edOrgService.GetEdOrgById(testEdOrgId, testSchoolYearId);

            result.ShouldNotBeNull();
        }
    }
}
