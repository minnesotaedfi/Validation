namespace ValidationWeb.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Moq;
    using NUnit.Framework;
    using ValidationWeb.Services;

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class AnnouncementServiceTests
    {
        protected MockRepository MockRepository { get; set; }

        protected Mock<IValidationPortalDbContext> ValidationPortalDbContextMock { get; set; }

        protected Mock<IAppUserService> AppUserServiceMock { get; set; }

        protected Mock<ILoggingService> LoggingServiceMock { get; set; }

        protected Mock<IAnnouncementService> AnnouncementServiceMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);

            ValidationPortalDbContextMock = MockRepository.Create<IValidationPortalDbContext>();
            AppUserServiceMock = MockRepository.Create<IAppUserService>();
            LoggingServiceMock = MockRepository.Create<ILoggingService>();
            AnnouncementServiceMock = MockRepository.Create<IAnnouncementService>();
        }

        [SetUp]
        public void SetUp()
        {
            var sessionId = 1234;

            var appUserSession =
                new AppUserSession
                {
                    FocusedEdOrgId = 1234,
                    FocusedSchoolYearId = 1,
                    Id = "234",
                    DismissedAnnouncements = null,
                    ExpiresUtc = DateTime.Now.AddMonths(1),
                    UserIdentity =
                            new ValidationPortalIdentity
                            {
                                AuthorizedEdOrgs =
                                    new List<EdOrg>(
                                        new[]
                                            {
                                                new EdOrg
                                                    {
                                                        Id = 1234
                                                    }
                                            })
                            }
                };

            var dismissedAnnouncements =
                new List<DismissedAnnouncement>(new[]
                {
                     new DismissedAnnouncement()
                         {
                             AppUserSession = appUserSession,
                             AnnouncementId = 2345,
                             AppUserSessionId = appUserSession.Id
                         }
                });

            AppUserServiceMock.Setup(x => x.GetSession()).Returns(appUserSession);


        }

        [Test]
        public void GetAnnouncements_Should_Return_NotLimitedOrDismissed()
        {
        }
    }
}
