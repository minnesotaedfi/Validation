
namespace ValidationWeb.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web;

    using Moq;
    using NUnit.Framework;

    using Should;

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
            DefaultTestAppUserSession.DismissedAnnouncements = new List<DismissedAnnouncement>();
        }

        [Test]
        public void DismissAnnouncement_Should_AddToDismissedAnnouncements()
        {
            var authorizedEdOrgId = 12345;
            DefaultTestAppUserSession.UserIdentity.AuthorizedEdOrgs.Add(new EdOrg { Id = authorizedEdOrgId });

            var announcementIdToDismiss = 2345;
            var announcementIdToKeep = 3456;

            var announcements = new List<Announcement>(
                new[]
                {
                    new Announcement
                    {
                        Id = announcementIdToKeep,
                        Message = "test message",
                        LimitToEdOrgs = new List<EdOrg>(new[] { new EdOrg { Id = authorizedEdOrgId } })
                    },
                    new Announcement
                    {
                        Id = announcementIdToDismiss,
                        Message = "dismiss me",
                        LimitToEdOrgs = new List<EdOrg>(new[] { new EdOrg { Id = authorizedEdOrgId } })
                    }
                });

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var httpContext = new HttpContext(
                new HttpRequest(string.Empty, "http://wearedoubleline.com", string.Empty),
                new HttpResponse(new StringWriter()));

            httpContext.Items[AppUserService.SessionItemName] = DefaultTestAppUserSession;

            HttpContextProviderMock.SetupGet(x => x.CurrentHttpContext).Returns(httpContext);
            
            var appUserService = new AppUserService(
                DbContextFactoryMock.Object,
                HttpContextProviderMock.Object,
                LoggingServiceMock.Object);

            appUserService.DismissAnnouncement(announcementIdToDismiss);

            DefaultTestAppUserSession.DismissedAnnouncements.Count(x => x.AnnouncementId == announcementIdToDismiss).ShouldEqual(1);
            ValidationPortalDbContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}
