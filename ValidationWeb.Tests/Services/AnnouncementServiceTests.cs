namespace ValidationWeb.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    using Moq;
    using NUnit.Framework;

    using Should;

    using ValidationWeb.Services;
    using ValidationWeb.Tests.Mocks;

    [TestFixture]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "tests don't follow standard naming")]
    public class AnnouncementServiceTests
    {
        protected MockRepository MockRepository { get; set; }

        protected Mock<ValidationPortalDbContext> ValidationPortalDbContextMock { get; set; }

        protected Mock<IDbContextFactory<ValidationPortalDbContext>> DbContextFactoryMock { get; set; }

        protected Mock<IAppUserService> AppUserServiceMock { get; set; }

        protected Mock<ILoggingService> LoggingServiceMock { get; set; }

        protected Mock<IAnnouncementService> AnnouncementServiceMock { get; set; }

        protected AppUserSession DefaultTestAppUserSession { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);

            ValidationPortalDbContextMock = MockRepository.Create<ValidationPortalDbContext>();
            DbContextFactoryMock = MockRepository.Create<IDbContextFactory<ValidationPortalDbContext>>();
            AppUserServiceMock = MockRepository.Create<IAppUserService>();
            LoggingServiceMock = MockRepository.Create<ILoggingService>();
            AnnouncementServiceMock = MockRepository.Create<IAnnouncementService>();
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
                UserIdentity = new ValidationPortalIdentity { AuthorizedEdOrgs = new List<EdOrg>() },
            };
        }

        [TearDown]
        public void TearDown()
        {
            ValidationPortalDbContextMock.Reset();
            DbContextFactoryMock.Reset();
            AnnouncementServiceMock.Reset();
            AppUserServiceMock.Reset();
            LoggingServiceMock.Reset();
        }

        [Test]
        public void GetAnnouncements_WithPreviousDismissedAnnouncementsTrue_Should_ReturnAll()
        {
            AppUserServiceMock.Setup(x => x.GetSession()).Returns(DefaultTestAppUserSession);

            // authorized ed orgs should match limited-to ed orgs 
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
                        Message = "test contact info",
                        LimitToEdOrgs = new List<EdOrg>(new[] { new EdOrg { Id = authorizedEdOrgId } })
                    },
                    new Announcement
                    {
                        Id = announcementIdToDismiss,
                        Message = "dismiss me",
                        LimitToEdOrgs = new List<EdOrg>(new[] { new EdOrg { Id = authorizedEdOrgId } })
                    }
                });

            var dismissedAnnouncements =
                new List<DismissedAnnouncement>
                {
                    new DismissedAnnouncement
                    {
                        AppUserSession = DefaultTestAppUserSession,
                        AnnouncementId = announcementIdToDismiss,
                        AppUserSessionId = DefaultTestAppUserSession.Id
                    }
                };

            DefaultTestAppUserSession.DismissedAnnouncements = dismissedAnnouncements;

            EntityFrameworkMocks.SetupMockDbSet<ValidationPortalDbContext, Announcement>(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var announcementService = new AnnouncementService(DbContextFactoryMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            var result = announcementService.GetAnnouncements(true);

            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.ShouldContain(announcements.First(x => x.Id == announcementIdToKeep));
            result.ShouldContain(announcements.First(x => x.Id == announcementIdToDismiss));
        }

        [Test]
        public void GetAnnouncements_WithPreviousDismissedAnnouncementsFalse_Should_ReturnOnlyNotDismissed()
        {
            AppUserServiceMock.Setup(x => x.GetSession()).Returns(DefaultTestAppUserSession);

            // authorized ed orgs should match limited-to ed orgs 
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

            var dismissedAnnouncements =
                new List<DismissedAnnouncement>
                {
                    new DismissedAnnouncement
                    {
                        AppUserSession = DefaultTestAppUserSession,
                        AnnouncementId = announcementIdToDismiss,
                        AppUserSessionId = DefaultTestAppUserSession.Id
                    }
                };

            DefaultTestAppUserSession.DismissedAnnouncements = dismissedAnnouncements;

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);
            
            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);
            var announcementService = new AnnouncementService(DbContextFactoryMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            var result = announcementService.GetAnnouncements(false);

            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.ShouldContain(announcements.First(x => x.Id == announcementIdToKeep));
            result.ShouldNotContain(announcements.First(x => x.Id == announcementIdToDismiss));
        }

        [Test]
        public void GetAnnouncements_Should_LogCaughtException()
        {
            AppUserServiceMock.Setup(x => x.GetSession()).Throws<InvalidOperationException>();
            LoggingServiceMock.Setup(x => x.LogErrorMessage(It.IsAny<string>()));
            var announcementService = new AnnouncementService(DbContextFactoryMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            announcementService.GetAnnouncements();

            LoggingServiceMock.Verify(x => x.LogErrorMessage(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetAnnouncement_Should_ReturnOneAnnouncement()
        {
            AppUserServiceMock.Setup(x => x.GetSession()).Returns(DefaultTestAppUserSession);

            // authorized ed orgs should match limited-to ed orgs 
            var authorizedEdOrgId = 12345;
            DefaultTestAppUserSession.UserIdentity.AuthorizedEdOrgs.Add(new EdOrg { Id = authorizedEdOrgId });

            var announcementIdToReturn = 2345;
            var announcementIdToNotReturn = 3456;

            var announcements = new List<Announcement>(
                new[]
                {
                    new Announcement
                    {
                        Id = announcementIdToReturn,
                        Message = "return me",
                        LimitToEdOrgs = new List<EdOrg>(new[] { new EdOrg { Id = authorizedEdOrgId } })
                    },
                    new Announcement
                    {
                        Id = announcementIdToNotReturn,
                        Message = "leave me out",
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

            var announcementService = new AnnouncementService(DbContextFactoryMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            var result = announcementService.GetAnnouncement(announcementIdToReturn);

            result.ShouldNotBeNull();
            result.Id.ShouldEqual(announcementIdToReturn);
        }

        [Test]
        public void GetAnnouncement_Should_LogAndThrowOnNonexistentId()
        {
            AppUserServiceMock.Setup(x => x.GetSession()).Returns(DefaultTestAppUserSession);
            LoggingServiceMock.Setup(x => x.LogErrorMessage(It.IsAny<string>()));

            // authorized ed orgs should match limited-to ed orgs 
            var authorizedEdOrgId = 12345;
            DefaultTestAppUserSession.UserIdentity.AuthorizedEdOrgs.Add(new EdOrg { Id = authorizedEdOrgId });

            var announcementIdToReturn = 2345;
            var announcementIdToNotReturn = 3456;
            var announcementIdThatsBogus = 4567;

            var announcements = new List<Announcement>(
                new[]
                {
                    new Announcement
                    {
                        Id = announcementIdToReturn,
                        Message = "return me",
                        LimitToEdOrgs = new List<EdOrg>(new[] { new EdOrg { Id = authorizedEdOrgId } })
                    },
                    new Announcement
                    {
                        Id = announcementIdToNotReturn,
                        Message = "leave me out",
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

            var announcementService = new AnnouncementService(DbContextFactoryMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            Assert.Throws<Exception>(() => announcementService.GetAnnouncement(announcementIdThatsBogus));

            LoggingServiceMock.Verify(x => x.LogErrorMessage(It.IsAny<string>()));
        }

        [Test]
        public void DeleteAnnouncement_Should_DeleteTheAnnouncement()
        {
            var announcement = new Announcement { Id = 12345, Message = "test contact info" };
            var announcements = new List<Announcement>(new[] { announcement });

            var announcementDbSetMock = EntityFrameworkMocks.GetQueryableMockDbSet(announcements);

            EntityFrameworkMocks.SetupMockDbSet(
                announcementDbSetMock, 
                ValidationPortalDbContextMock, 
                x => x.Announcements, 
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);
            
            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var announcementService = new AnnouncementService(DbContextFactoryMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            announcementService.DeleteAnnouncement(announcement.Id);

            announcementDbSetMock.Verify(x => x.Remove(announcement), Times.Once);
            ValidationPortalDbContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
        public void SaveNewAnnouncement_Should_SaveAnnouncement()
        {
            var announcement =
                new Announcement
                {
                    Id = 0,
                    Priority = 1,
                    Message = "test message",
                    ContactInfo = "3-2-1, contact",
                    LinkUrl = "http://wearedoubleline.com",
                    Expiration = DateTime.Now.AddMonths(1)
                };

            var announcements = new List<Announcement>(new[] { announcement });

            var dbSetMock = EntityFrameworkMocks.GetQueryableMockDbSet(announcements);

            EntityFrameworkMocks.SetupMockDbSet<ValidationPortalDbContext, Announcement>(
                dbSetMock,
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var announcementService = new AnnouncementService(
                DbContextFactoryMock.Object,
                AppUserServiceMock.Object,
                LoggingServiceMock.Object);

            announcementService.SaveAnnouncement(
                announcement.Id,
                announcement.Priority,
                announcement.Message,
                announcement.ContactInfo,
                announcement.LinkUrl,
                announcement.Expiration);

            dbSetMock.Verify(x => x.Add(It.Is<Announcement>(y => y.Id == announcement.Id)), Times.Once);
            ValidationPortalDbContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
        public void SaveExistingAnnouncement_Should_SaveAnnouncement()
        {
            var announcement =
                new Announcement
                {
                    Id = 1234,
                    Priority = 1,
                    Message = "test message",
                    ContactInfo = "3-2-1, contact",
                    LinkUrl = "http://wearedoubleline.com",
                    Expiration = DateTime.Now.AddMonths(1)
                };

            var announcements = new List<Announcement>(new[] { announcement });

            EntityFrameworkMocks.SetupMockDbSet<ValidationPortalDbContext, Announcement>(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var announcementService = new AnnouncementService(DbContextFactoryMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            announcementService.SaveAnnouncement(
                announcement.Id,
                announcement.Priority,
                announcement.Message,
                announcement.ContactInfo,
                announcement.LinkUrl,
                announcement.Expiration);

            ValidationPortalDbContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }
        
        [Test]
        public void SaveExistingAnnouncement_Should_ThrowIfNotExistsAndLogError()
        {
            var announcement =
                new Announcement
                {
                    Id = 1234,
                    Priority = 1,
                    Message = "test message",
                    ContactInfo = "3-2-1, contact",
                    LinkUrl = "http://wearedoubleline.com",
                    Expiration = DateTime.Now.AddMonths(1)
                };
            
            var announcements = new List<Announcement>(new[] { announcement });
            
            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            EntityFrameworkMocks.SetupMockDbSet<ValidationPortalDbContext, Announcement>(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            LoggingServiceMock.Setup(x => x.LogErrorMessage(It.IsAny<string>()));

            var announcementService = new AnnouncementService(DbContextFactoryMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            Assert.Throws<Exception>(() =>
                announcementService.SaveAnnouncement(
                    announcement.Id + 1,
                    announcement.Priority,
                    announcement.Message,
                    announcement.ContactInfo,
                    announcement.LinkUrl,
                    announcement.Expiration));
        }

        [Test]
        public void DeleteAnnouncement_Should_ThrowOnNonexistentAnnouncement()
        {
            var announcement = new Announcement { Id = 12345, Message = "test contact info" };
            var announcements = new List<Announcement>(new[] { announcement });

            var announcementDbSetMock = EntityFrameworkMocks.GetQueryableMockDbSet(announcements);
            
            EntityFrameworkMocks.SetupMockDbSet<ValidationPortalDbContext, Announcement>(
                announcementDbSetMock,
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);
            
            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);
            
            var announcementService = new AnnouncementService(DbContextFactoryMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            var badAnnouncementId = announcement.Id + 1;
            Assert.Throws(
                Is.TypeOf<Exception>().And.Message.EqualTo($"Could not delete an announcement because announcement with ID {badAnnouncementId} was not found"),
                () => announcementService.DeleteAnnouncement(badAnnouncementId));
        }
    }
}
