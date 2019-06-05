using Moq;
using NUnit.Framework;
using Should;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Implementations;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Tests.Mocks;

namespace ValidationWeb.Tests.Services
{
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
        public void GetAnnouncements_Should_ReturnAll()
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
                        Message = "test contact info"
                    },
                    new Announcement
                    {
                        Id = announcementIdToDismiss,
                        Message = "dismiss me"
                    }
                });

            EntityFrameworkMocks.SetupMockDbSet<ValidationPortalDbContext, Announcement>(
                EntityFrameworkMocks.GetQueryableMockDbSet(announcements),
                ValidationPortalDbContextMock,
                x => x.Announcements,
                x => x.Announcements = It.IsAny<DbSet<Announcement>>(),
                announcements);

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);

            var announcementService = new AnnouncementService(DbContextFactoryMock.Object, AppUserServiceMock.Object, LoggingServiceMock.Object);

            var result = announcementService.GetAnnouncements();

            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.ShouldContain(announcements.First(x => x.Id == announcementIdToKeep));
            result.ShouldContain(announcements.First(x => x.Id == announcementIdToDismiss));
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
                    },
                    new Announcement
                    {
                        Id = announcementIdToNotReturn,
                        Message = "leave me out",
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
                        Message = "return me"
                    },
                    new Announcement
                    {
                        Id = announcementIdToNotReturn,
                        Message = "leave me out"
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
