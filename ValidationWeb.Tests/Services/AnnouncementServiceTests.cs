namespace ValidationWeb.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Moq;
    using NUnit.Framework;

    using Should;

    using ValidationWeb.Services;

    [TestFixture]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "tests don't follow standard naming")]
    public class AnnouncementServiceTests
    {
        protected MockRepository MockRepository { get; set; }

        protected Mock<ValidationPortalDbContext> ValidationPortalDbContextMock { get; set; }

        protected Mock<IAppUserService> AppUserServiceMock { get; set; }

        protected Mock<ILoggingService> LoggingServiceMock { get; set; }

        protected Mock<IAnnouncementService> AnnouncementServiceMock { get; set; }

        protected AppUserSession DefaultTestAppUserSession { get; set; }

        public Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = MockRepository.Create<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(sourceList.Add);

            return dbSet;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);

            ValidationPortalDbContextMock = MockRepository.Create<ValidationPortalDbContext>();
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
                UserIdentity = new ValidationPortalIdentity {AuthorizedEdOrgs = new List<EdOrg>() },
            };
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
            var announcementDbSetMock = GetQueryableMockDbSet<Announcement>(announcements);
            ValidationPortalDbContextMock.Setup(x => x.Announcements).Returns(announcementDbSetMock.Object);

            var announcementService = new AnnouncementService(ValidationPortalDbContextMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            var result = announcementService.GetAnnoucements(true);

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
            var announcementDbSetMock = GetQueryableMockDbSet<Announcement>(announcements);
            ValidationPortalDbContextMock.Setup(x => x.Announcements).Returns(announcementDbSetMock.Object);

            var announcementService = new AnnouncementService(ValidationPortalDbContextMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            var result = announcementService.GetAnnoucements(true);

            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.ShouldContain(announcements.First(x => x.Id == announcementIdToKeep));
            result.ShouldNotContain(announcements.First(x => x.Id == announcementIdToDismiss));
        }

        [Test]
        public void DeleteAnnouncement_Should_DeleteTheAnnouncement()
        {
            var announcement = new Announcement { Id = 12345, Message = "test contact info" };

            var announcementDbSetMock = GetQueryableMockDbSet(new List<Announcement>(new[] { announcement }));
            announcementDbSetMock.Setup(x => x.Remove(It.Is<Announcement>(y => y == announcement))).Returns(announcement);

            ValidationPortalDbContextMock
                .Setup(x => x.Announcements)
                .Returns(announcementDbSetMock.Object);

            ValidationPortalDbContextMock.Setup(x => x.SaveChanges()).Returns(1);

            var announcementService = new AnnouncementService(ValidationPortalDbContextMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            announcementService.DeleteAnnoucement(announcement.Id);

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

            var announcementDbSetMock = GetQueryableMockDbSet(new List<Announcement>(new[] { announcement }));
            announcementDbSetMock.Setup(x => x.Add(It.Is<Announcement>(y => y.Message == announcement.Message))).Returns(announcement);
            announcementDbSetMock.Setup(x => x.ToString()).Returns("bobby tables");

            ValidationPortalDbContextMock
                .Setup(x => x.Announcements)
                .Returns(announcementDbSetMock.Object);

            ValidationPortalDbContextMock.Setup(x => x.Set<Announcement>()).Returns(() => announcementDbSetMock.Object);

            ValidationPortalDbContextMock.Setup(x => x.SaveChanges()).Returns(1);

            var announcementService = new AnnouncementService(ValidationPortalDbContextMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            announcementService.SaveAnnouncement(
                announcement.Id,
                announcement.Priority,
                announcement.Message,
                announcement.ContactInfo,
                announcement.LinkUrl,
                announcement.Expiration);

            announcementDbSetMock.Verify(x => x.Add(It.Is<Announcement>(y => y.Id == announcement.Id)), Times.Once);
            ValidationPortalDbContextMock.Verify(x => x.SaveChanges());
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

            var announcementDbSetMock = GetQueryableMockDbSet(new List<Announcement>(new[] { announcement }));

            ValidationPortalDbContextMock
                .Setup(x => x.Announcements)
                .Returns(announcementDbSetMock.Object);

            ValidationPortalDbContextMock.Setup(x => x.SaveChanges()).Returns(1);

            var announcementService = new AnnouncementService(ValidationPortalDbContextMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

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

            var announcementDbSetMock = GetQueryableMockDbSet(new List<Announcement>(new[] { announcement }));

            ValidationPortalDbContextMock
                .Setup(x => x.Announcements)
                .Returns(announcementDbSetMock.Object);

            ValidationPortalDbContextMock.Setup(x => x.SaveChanges()).Returns(1);

            LoggingServiceMock.Setup(x => x.LogErrorMessage(It.IsAny<string>()));

            var announcementService = new AnnouncementService(ValidationPortalDbContextMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            Assert.Throws<Exception>( () => 
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

            var announcementDbSetMock = GetQueryableMockDbSet(new List<Announcement>(new[] { announcement }));
            announcementDbSetMock.Setup(x => x.Remove(It.Is<Announcement>(y => y == announcement))).Returns(announcement);

            ValidationPortalDbContextMock
                .Setup(x => x.Announcements)
                .Returns(announcementDbSetMock.Object);

            ValidationPortalDbContextMock.Setup(x => x.SaveChanges()).Returns(1);

            var announcementService = new AnnouncementService(ValidationPortalDbContextMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);
            var badAnnouncementId = announcement.Id + 1;
            Assert.Throws(
                Is.TypeOf<Exception>().And.Message.EqualTo($"Could not delete an announcement because announcement with ID {badAnnouncementId} was not found"), 
                () => announcementService.DeleteAnnoucement(badAnnouncementId));
        }
    }
}
