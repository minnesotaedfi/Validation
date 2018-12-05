namespace ValidationWeb.Tests.Services
{
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

        protected AnnouncementService AnnouncementService { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);

            ValidationPortalDbContextMock = MockRepository.Create<IValidationPortalDbContext>();
            AppUserServiceMock = MockRepository.Create<IAppUserService>();
            LoggingServiceMock = MockRepository.Create<ILoggingService>();
        }

        [SetUp]
        public void SetUp()
        {
            AnnouncementService = new AnnouncementService(
                ValidationPortalDbContextMock.Object, 
                AppUserServiceMock.Object, 
                LoggingServiceMock.Object);
        }

        [Test]
        public void GetAnnouncements_Should_Return_NotLimitedOrDismissed()
        {
            var announcements = AnnouncementService.GetAnnoucements(false);
        }
    }
}
