
namespace ValidationWeb.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Moq;
    using NUnit.Framework;

    using Should;
    using Should.Extensions;

    using ValidationWeb.Database;
    using ValidationWeb.Filters;
    using ValidationWeb.Services;
    using ValidationWeb.Tests.Mocks;

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class EdOrgServiceTests
    {
        protected MockRepository MockRepository { get; set; }

        protected AppUserSession DefaultTestAppUserSession { get; set; }

        protected Mock<ValidationPortalDbContext> ValidationPortalDbContextMock { get; set; }
        
        protected Mock<RawOdsDbContext> RawOdsDbContextMock { get; set; }

        protected Mock<IAppUserService> AppUserServiceMock { get; set; }

        protected Mock<ISchoolYearService> SchoolYearServiceMock { get; set; }
        
        protected Mock<IDbContextFactory<ValidationPortalDbContext>> DbContextFactoryMock { get; set; }

        protected Mock<ICustomDbContextFactory<RawOdsDbContext>> CustomDbContextFactoryMock { get; set; }

        protected Mock<ILoggingService> LoggingServiceMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);

            ValidationPortalDbContextMock = MockRepository.Create<ValidationPortalDbContext>(MockBehavior.Loose);
            RawOdsDbContextMock = MockRepository.Create<RawOdsDbContext>(MockBehavior.Loose);
            AppUserServiceMock = MockRepository.Create<IAppUserService>();
            SchoolYearServiceMock = MockRepository.Create<ISchoolYearService>();
            DbContextFactoryMock = MockRepository.Create<IDbContextFactory<ValidationPortalDbContext>>();
            LoggingServiceMock = MockRepository.Create<ILoggingService>();
            CustomDbContextFactoryMock = MockRepository.Create<ICustomDbContextFactory<RawOdsDbContext>>();
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
        }

        [TearDown]
        public void TearDown()
        {
            ValidationPortalDbContextMock.Reset();
            DbContextFactoryMock.Reset();
            LoggingServiceMock.Reset();
            RawOdsDbContextMock.Reset();
            AppUserServiceMock.Reset();
            SchoolYearServiceMock.Reset();
            CustomDbContextFactoryMock.Reset();
        }

        [Test]
        public void GetAuthorizedEdOrgs_Should_ReturnUserSessionEdOrgsOrdered()
        {
            var testAppUserSession = new AppUserSession
            {
                FocusedEdOrgId = 1234,
                FocusedSchoolYearId = 1,
                Id = "234",
                ExpiresUtc = DateTime.Now.AddMonths(1),
                UserIdentity = new ValidationPortalIdentity
                               {
                                   AuthorizedEdOrgs = new List<EdOrg>(
                                       new[]
                                                 {
                                                    new EdOrg { Id = 1234, OrganizationName = "zzzz" },
                                                    new EdOrg { Id = 2345, OrganizationName = "yyyy" },
                                                    new EdOrg { Id = 3456, OrganizationName = "xxxx" },
                                                    new EdOrg { Id = 4567, OrganizationName = "wwww" },
                                                 })
                               },
            };

            AppUserServiceMock.Setup(x => x.GetSession()).Returns(testAppUserSession);

            var edOrgService = new EdOrgService(
                DbContextFactoryMock.Object,
                CustomDbContextFactoryMock.Object,
                AppUserServiceMock.Object,
                SchoolYearServiceMock.Object,
                LoggingServiceMock.Object);

            var result = edOrgService.GetAuthorizedEdOrgs();
            
            result.ShouldNotBeNull(); 
            result.ShouldNotBeEmpty();
            result.ShouldEqual(testAppUserSession.UserIdentity.AuthorizedEdOrgs.OrderBy(x => x.OrganizationName).ToList());
        }

        [Test]
        public void GetAllEdOrgs_Should_ReturnEdOrgs()
        {
            var edOrgs = new List<EdOrg>(new[]
                                         {
                                             new EdOrg { Id = 1234, OrganizationName = "zzzz" },
                                             new EdOrg { Id = 2345, OrganizationName = "yyyy" },
                                             new EdOrg { Id = 3456, OrganizationName = "xxxx" },
                                             new EdOrg { Id = 4567, OrganizationName = "wwww" },
                                         });

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(edOrgs),
                ValidationPortalDbContextMock,
                x => x.EdOrgs,
                x => x.EdOrgs = It.IsAny<DbSet<EdOrg>>(),
                edOrgs);

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);
            
            var edOrgService = new EdOrgService(
                DbContextFactoryMock.Object,
                CustomDbContextFactoryMock.Object,
                AppUserServiceMock.Object,
                SchoolYearServiceMock.Object,
                LoggingServiceMock.Object);

            var result = edOrgService.GetAllEdOrgs();

            result.ShouldEqual(edOrgs);
        }

        [Test]
        public void GetEdOrgById_Should_RefreshEdOrgCache()
        {
            var testSchoolYearId = 234;
            var testSchoolYear = new SchoolYear
                                 {
                                     StartYear = "2016",
                                     EndYear = "2017",
                                     Enabled = true,
                                     ErrorThreshold = null,
                                     Id = testSchoolYearId
                                 };

            SchoolYearServiceMock
                .Setup(x => x.GetSchoolYearById(testSchoolYearId))
                .Returns(testSchoolYear);

            var testEdOrgId = 123;
            var validationPortalEdOrgs = new List<EdOrg>();

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(validationPortalEdOrgs),
                ValidationPortalDbContextMock,
                x => x.EdOrgs,
                x => x.EdOrgs = It.IsAny<DbSet<EdOrg>>(),
                validationPortalEdOrgs);
                        
            ValidationPortalDbContextMock.Setup(x => x.AddOrUpdate(It.IsAny<EdOrg>()));

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var odsEdOrgs = new[]
            {
                new EdOrg { Id = 1234, OrganizationName = "zzzz" },
                new EdOrg { Id = 2345, OrganizationName = "yyyy" },
                new EdOrg { Id = 3456, OrganizationName = "xxxx" },
                new EdOrg { Id = testEdOrgId, OrganizationName = "wwww" },
            };
            
            var connectionMock = MockRepository.Create<DbConnection>();
            connectionMock.SetupGet(x => x.State).Returns(ConnectionState.Closed);
            connectionMock.Setup(x => x.Open());

            var commandMock = MockRepository.Create<IDbCommand>();
            commandMock.SetupSet(x => x.CommandType = CommandType.Text);
            commandMock.SetupSet(x => x.CommandText = EdOrgQuery.AllEdOrgQuery);

            var readerMock = MockRepository.Create<IDataReader>();
            readerMock.Setup(x => x.Dispose());
            
            var readerIndex = 0;
            readerMock
                .Setup(x => x.Read())
                .Returns(() => readerIndex <= odsEdOrgs.Length)
                .Callback(() => readerIndex++);

            readerMock.SetupGet(x => x[EdOrgQuery.IdColumnName]).Returns(odsEdOrgs[readerIndex].Id);
            readerMock.SetupGet(x => x[EdOrgQuery.OrganizationNameColumnName]).Returns(odsEdOrgs[readerIndex].OrganizationName);
            readerMock.SetupGet(x => x[EdOrgQuery.StateOrganizationIdColumnName]).Returns(null);
            readerMock.SetupGet(x => x[EdOrgQuery.StateLevelOrganizationIdColumnName]).Returns(null);
            readerMock.SetupGet(x => x[EdOrgQuery.OrganizationShortNameColumnName]).Returns(string.Empty);
            readerMock.SetupGet(x => x[EdOrgQuery.ParentIdColumnName]).Returns(null);
            commandMock.Setup(x => x.ExecuteReader()).Returns(readerMock.Object);
            
            RawOdsDbContextMock.Setup(x => x.CreateCommand()).Returns(commandMock.Object);
            RawOdsDbContextMock.Setup(x => x.Connection).Returns(connectionMock.Object);
            
            CustomDbContextFactoryMock
                .Setup(x => x.Create(testSchoolYear.EndYear))
                .Returns(RawOdsDbContextMock.Object);

            LoggingServiceMock.Setup(x => x.LogDebugMessage(It.IsAny<string>()));

            var edOrgService = new EdOrgService(
                DbContextFactoryMock.Object,
                CustomDbContextFactoryMock.Object,
                AppUserServiceMock.Object,
                SchoolYearServiceMock.Object,
                LoggingServiceMock.Object);
            
            var result = edOrgService.GetEdOrgById(testEdOrgId, testSchoolYearId);
        }
    }
}
