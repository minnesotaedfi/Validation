using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    public class SchoolYearServiceTests
    {
        protected MockRepository MockRepository { get; set; }

        protected Mock<ValidationPortalDbContext> ValidationPortalDbContextMock { get; set; }

        protected Mock<IDbContextFactory<ValidationPortalDbContext>> DbContextFactoryMock { get; set; }

        protected Mock<ILoggingService> LoggingServiceMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);

            ValidationPortalDbContextMock = MockRepository.Create<ValidationPortalDbContext>();
            DbContextFactoryMock = MockRepository.Create<IDbContextFactory<ValidationPortalDbContext>>();
            LoggingServiceMock = MockRepository.Create<ILoggingService>();
        }

        [SetUp]
        public void SetUp()
        {
            EntityFrameworkMocks.SetupValidationPortalDbContext(ValidationPortalDbContextMock);

            TestSchoolYears = new List<SchoolYear>(new[]
            {
                new SchoolYear("2017", "2018", false) { Id = 0 },
                new SchoolYear("2018", "2019", true) { Id = 1 },
                new SchoolYear("2019", "2020", true) { Id = 2 }, 
            }); 

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(TestSchoolYears),
                ValidationPortalDbContextMock,
                x => x.SchoolYears,
                x => x.SchoolYears = It.IsAny<DbSet<SchoolYear>>(),
                TestSchoolYears);

            ValidationPortalDbContextMock.As<IDisposable>().Setup(x => x.Dispose());

            DbContextFactoryMock.Setup(x => x.Create()).Returns(ValidationPortalDbContextMock.Object);
        }

        public List<SchoolYear> TestSchoolYears { get; set; }

        [TearDown]
        public void TearDown()
        {
            ValidationPortalDbContextMock.Reset();
            DbContextFactoryMock.Reset();
            LoggingServiceMock.Reset();
        }
        
        [Test]
        public void GetSubmittableSchoolYears_Should_ReturnAllEnabled()
        {
            var schoolYearService = new SchoolYearService(DbContextFactoryMock.Object);

            var result = schoolYearService.GetSubmittableSchoolYears();

            result.ShouldEqual(TestSchoolYears);
        }

        [Test]
        public void GetSubmittableSchoolYearsDictionary_Should_BeADictionary()
        {
            var schoolYearService = new SchoolYearService(DbContextFactoryMock.Object);

            var result = schoolYearService.GetSubmittableSchoolYearsDictionary();

            result.ShouldBeType(typeof(Dictionary<int, string>));
            result.Values.ShouldEqual(TestSchoolYears.Select(x => $"{x.StartYear}-{x.EndYear}"));
        }
    }
}
