
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

    using ValidationWeb.Filters;
    using ValidationWeb.Services;
    using ValidationWeb.Tests.Mocks;

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class AppUserServiceTests
    {
        protected MockRepository MockRepository { get; set; }

        protected AppUserSession DefaultTestAppUserSession { get; set; }

        protected Mock<ValidationPortalDbContext> ValidationPortalDbContextMock { get; set; }

        protected Mock<IDbContextFactory<ValidationPortalDbContext>> DbContextFactoryMock { get; set; }

        protected Mock<IHttpContextProvider> HttpContextProviderMock { get; set; }

        protected Mock<ILoggingService> LoggingServiceMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);

            ValidationPortalDbContextMock = MockRepository.Create<ValidationPortalDbContext>();
            DbContextFactoryMock = MockRepository.Create<IDbContextFactory<ValidationPortalDbContext>>();
            HttpContextProviderMock = MockRepository.Create<IHttpContextProvider>();
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
            HttpContextProviderMock.Reset();
            LoggingServiceMock.Reset();
        }
        
        [Test]
        public void GetUser_Should_ReturnUser()
        {
            var httpContext = new HttpContext(
                new HttpRequest(string.Empty, "http://wearedoubleline.com", string.Empty),
                new HttpResponse(new StringWriter()));

            httpContext.Items[AppUserService.SessionItemName] = DefaultTestAppUserSession;

            HttpContextProviderMock.SetupGet(x => x.CurrentHttpContext).Returns(httpContext);

            var appUserService = new AppUserService(
                DbContextFactoryMock.Object,
                HttpContextProviderMock.Object,
                LoggingServiceMock.Object);

            var result = appUserService.GetUser();

            result.ShouldEqual(DefaultTestAppUserSession.UserIdentity);
        }

        [Test]
        public void UpdateFocusedEdOrg_Should_FailSilentlyBecauseItJustDoes()
        {
            var authorizedEdOrgId = 12345;
            DefaultTestAppUserSession.UserIdentity.AuthorizedEdOrgs.Add(new EdOrg { Id = authorizedEdOrgId });

            var announcements = new List<Announcement>();
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            var oldFocusedEdOrg = 12345;
            var newFocusedEdOrg = 23456;
            var randomAuthorizedEdOrg = newFocusedEdOrg + 1;
            var appUserSessionId = 123;

            var userIdentity = new ValidationPortalIdentity
            {
                AuthorizedEdOrgs = new List<EdOrg>(new[] { new EdOrg { Id = randomAuthorizedEdOrg } })
            };

            var appUserSession = new AppUserSession
            {
                Id = appUserSessionId.ToString(),
                FocusedEdOrgId = oldFocusedEdOrg,
                UserIdentity = userIdentity
            };

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var httpContext = new HttpContext(
                new HttpRequest(string.Empty, "http://wearedoubleline.com", string.Empty),
                new HttpResponse(new StringWriter()));

            httpContext.Items[AppUserService.SessionItemName] = appUserSession;
            HttpContextProviderMock.SetupGet(x => x.CurrentHttpContext).Returns(httpContext);

            LoggingServiceMock.Setup(x => x.LogDebugMessage(It.IsAny<string>()));

            var appUserService = new AppUserService(
                DbContextFactoryMock.Object,
                HttpContextProviderMock.Object,
                LoggingServiceMock.Object);

            appUserService.UpdateFocusedEdOrg(newFocusedEdOrg.ToString());

            ValidationPortalDbContextMock.Verify(x => x.SaveChanges(), Times.Never);
        }

        [Test]
        public void UpdateFocusedEdOrg_Should_LogException()
        {
            var authorizedEdOrgId = 12345;

            var announcements = new List<Announcement>();
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            var oldFocusedEdOrg = 12345;
            var newFocusedEdOrg = 23456;
            var appUserSessionId = 123;

            var appUserSession = new AppUserSession
            {
                Id = appUserSessionId.ToString(),
                FocusedEdOrgId = oldFocusedEdOrg,
                UserIdentity = null
            };

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var httpContext = new HttpContext(
                new HttpRequest(string.Empty, "http://wearedoubleline.com", string.Empty),
                new HttpResponse(new StringWriter()));

            httpContext.Items[AppUserService.SessionItemName] = appUserSession;
            HttpContextProviderMock.SetupGet(x => x.CurrentHttpContext).Returns(httpContext);

            LoggingServiceMock.Setup(x => x.LogDebugMessage(It.IsAny<string>()));
            LoggingServiceMock.Setup(x => x.LogErrorMessage(It.IsAny<string>()));

            var appUserService = new AppUserService(
                DbContextFactoryMock.Object,
                HttpContextProviderMock.Object,
                LoggingServiceMock.Object);

            appUserService.UpdateFocusedEdOrg(newFocusedEdOrg.ToString());

            ValidationPortalDbContextMock.Verify(x => x.SaveChanges(), Times.Never);
            LoggingServiceMock.Verify(x => x.LogErrorMessage(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void UpdateFocusedEdOrg_Should_UpdateDataContext()
        {
            var announcements = new List<Announcement>();
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            var oldFocusedEdOrg = 12345;
            var newFocusedEdOrg = 23456;
            var appUserSessionId = 123;

            var userIdentity = new ValidationPortalIdentity
            {
                AuthorizedEdOrgs = new List<EdOrg>(new[]
                                                                      {
                                                                          new EdOrg { Id = oldFocusedEdOrg },
                                                                          new EdOrg { Id = newFocusedEdOrg }
                                                                      })
            };

            var appUserSession = new AppUserSession
            {
                Id = appUserSessionId.ToString(),
                FocusedEdOrgId = oldFocusedEdOrg,
                UserIdentity = userIdentity
            };

            var userSessions = new List<AppUserSession>(new[] { appUserSession });

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(userSessions),
                ValidationPortalDbContextMock,
                x => x.AppUserSessions,
                x => x.AppUserSessions = It.IsAny<DbSet<AppUserSession>>(),
                userSessions);

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var httpContext = new HttpContext(
                new HttpRequest(string.Empty, "http://wearedoubleline.com", string.Empty),
                new HttpResponse(new StringWriter()));

            httpContext.Items[AppUserService.SessionItemName] = appUserSession;
            HttpContextProviderMock.SetupGet(x => x.CurrentHttpContext).Returns(httpContext);

            LoggingServiceMock.Setup(x => x.LogDebugMessage(It.IsAny<string>()));
            LoggingServiceMock.Setup(x => x.LogErrorMessage(It.IsAny<string>()));

            var appUserService = new AppUserService(
                DbContextFactoryMock.Object,
                HttpContextProviderMock.Object,
                LoggingServiceMock.Object);

            appUserService.UpdateFocusedEdOrg(newFocusedEdOrg.ToString());

            ValidationPortalDbContextMock.Verify(x => x.SaveChanges(), Times.Once);
            LoggingServiceMock.Verify(x => x.LogErrorMessage(It.IsAny<string>()), Times.Never);
            ValidationPortalDbContextMock.Object.AppUserSessions.First().FocusedEdOrgId.ShouldEqual(newFocusedEdOrg);
            appUserSession.FocusedEdOrgId.ShouldEqual(newFocusedEdOrg);
        }

        // focused school year
        [Test]
        public void UpdateFocusedSchoolYear_Should_FailSilentlyBecauseItJustDoes()
        {
            var announcements = new List<Announcement>();
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            var oldFocusedSchoolYear = 12345;
            var newFocusedSchoolYear = 23456;
            var appUserSessionId = 123;

            var schoolYears = new List<SchoolYear>(new[]
                                                   {
                                                       new SchoolYear
                                                       {
                                                           Id = newFocusedSchoolYear,
                                                           Enabled = true
                                                       }
                                                   });

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(schoolYears),
                ValidationPortalDbContextMock,
                x => x.SchoolYears,
                x => x.SchoolYears = It.IsAny<DbSet<SchoolYear>>(),
                schoolYears);

            var userIdentity = new ValidationPortalIdentity();

            var appUserSession = new AppUserSession
            {
                Id = appUserSessionId.ToString(),
                FocusedSchoolYearId = oldFocusedSchoolYear,
                UserIdentity = userIdentity
            };

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var httpContext = new HttpContext(
                new HttpRequest(string.Empty, "http://wearedoubleline.com", string.Empty),
                new HttpResponse(new StringWriter()));

            httpContext.Items[AppUserService.SessionItemName] = null;
            HttpContextProviderMock.SetupGet(x => x.CurrentHttpContext).Returns(httpContext);

            LoggingServiceMock.Setup(x => x.LogDebugMessage(It.IsAny<string>()));

            var appUserService = new AppUserService(
                DbContextFactoryMock.Object,
                HttpContextProviderMock.Object,
                LoggingServiceMock.Object);

            appUserService.UpdateFocusedSchoolYear(newFocusedSchoolYear);

            ValidationPortalDbContextMock.Verify(x => x.SaveChanges(), Times.Never);
        }

        [Test]
        public void UpdateFocusedSchoolYear_Should_LogException()
        {
            var authorizedEdOrgId = 12345;

            var announcements = new List<Announcement>();
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            var newFocusedSchoolYear = 23456;
            var appUserSessionId = 123;

            var appUserSession = new AppUserSession
            {
                Id = appUserSessionId.ToString(),
                UserIdentity = null
            };

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var httpContext = new HttpContext(
                new HttpRequest(string.Empty, "http://wearedoubleline.com", string.Empty),
                new HttpResponse(new StringWriter()));

            httpContext.Items[AppUserService.SessionItemName] = appUserSession;
            HttpContextProviderMock.SetupGet(x => x.CurrentHttpContext).Returns(httpContext);

            LoggingServiceMock.Setup(x => x.LogDebugMessage(It.IsAny<string>()));
            LoggingServiceMock.Setup(x => x.LogErrorMessage(It.IsAny<string>()));

            var appUserService = new AppUserService(
                DbContextFactoryMock.Object,
                HttpContextProviderMock.Object,
                LoggingServiceMock.Object);

            appUserService.UpdateFocusedSchoolYear(newFocusedSchoolYear);

            ValidationPortalDbContextMock.Verify(x => x.SaveChanges(), Times.Never);
            LoggingServiceMock.Verify(x => x.LogErrorMessage(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void UpdateFocusedSchoolYear_Should_UpdateDataContext()
        {
            var announcements = new List<Announcement>();
            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            var oldFocusedSchoolYear = 12345;
            var newFocusedSchoolYear = 23456;
            var appUserSessionId = 123;

            var schoolYears = new List<SchoolYear>(new[]
                                                   {
                                                       new SchoolYear
                                                       {
                                                           Id = newFocusedSchoolYear,
                                                           Enabled = true
                                                       }
                                                   });

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(schoolYears),
                ValidationPortalDbContextMock,
                x => x.SchoolYears,
                x => x.SchoolYears = It.IsAny<DbSet<SchoolYear>>(),
                schoolYears);
            
            var appUserSession = new AppUserSession
            {
                Id = appUserSessionId.ToString(),
                FocusedSchoolYearId = oldFocusedSchoolYear
            };

            var userSessions = new List<AppUserSession>(new[] { appUserSession });

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(userSessions),
                ValidationPortalDbContextMock,
                x => x.AppUserSessions,
                x => x.AppUserSessions = It.IsAny<DbSet<AppUserSession>>(),
                userSessions);

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var httpContext = new HttpContext(
                new HttpRequest(string.Empty, "http://wearedoubleline.com", string.Empty),
                new HttpResponse(new StringWriter()));

            httpContext.Items[AppUserService.SessionItemName] = appUserSession;
            HttpContextProviderMock.SetupGet(x => x.CurrentHttpContext).Returns(httpContext);

            LoggingServiceMock.Setup(x => x.LogDebugMessage(It.IsAny<string>()));
            LoggingServiceMock.Setup(x => x.LogErrorMessage(It.IsAny<string>()));

            var appUserService = new AppUserService(
                DbContextFactoryMock.Object,
                HttpContextProviderMock.Object,
                LoggingServiceMock.Object);

            appUserService.UpdateFocusedSchoolYear(newFocusedSchoolYear);

            ValidationPortalDbContextMock.Verify(x => x.SaveChanges(), Times.Once);
            LoggingServiceMock.Verify(x => x.LogErrorMessage(It.IsAny<string>()), Times.Never);
            ValidationPortalDbContextMock.Object.AppUserSessions.First().FocusedSchoolYearId.ShouldEqual(newFocusedSchoolYear);
        }
    }
}
