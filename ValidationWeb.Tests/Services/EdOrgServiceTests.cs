
namespace ValidationWeb.Tests.Services
{
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
        
        protected Mock<IAppUserService> AppUserServiceMock { get; set; }

        protected Mock<ISchoolYearService> SchoolYearServiceMock { get; set; }
        
        protected Mock<IDbContextFactory<ValidationPortalDbContext>> DbContextFactoryMock { get; set; }

        protected Mock<ILoggingService> LoggingServiceMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);

            ValidationPortalDbContextMock = MockRepository.Create<ValidationPortalDbContext>();
            AppUserServiceMock = MockRepository.Create<IAppUserService>();
            SchoolYearServiceMock = MockRepository.Create<ISchoolYearService>();
            DbContextFactoryMock = MockRepository.Create<IDbContextFactory<ValidationPortalDbContext>>();
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
        }

        [TearDown]
        public void TearDown()
        {
            ValidationPortalDbContextMock.Reset();
            DbContextFactoryMock.Reset();
            LoggingServiceMock.Reset();
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
                null,
                SchoolYearServiceMock.Object,
                LoggingServiceMock.Object);

            var result = edOrgService.GetAllEdOrgs();

            result.ShouldEqual(edOrgs);
        }
    }
}
