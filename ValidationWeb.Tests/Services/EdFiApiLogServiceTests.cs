using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;

using Moq;

using NUnit.Framework;

using Should;
using Should.Extensions;

using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Implementations;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Tests.Mocks;

// ReSharper disable PossibleMultipleEnumeration
namespace ValidationWeb.Tests.Services
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class EdFiApiLogServiceTests
    {
        protected MockRepository MockRepository { get; set; }

        protected Mock<IDbContextFactory<EdFiLogDbContext>> DbContextFactoryMock { get; set; }

        protected Mock<EdFiLogDbContext> DbContextMock { get; set; }

        protected Mock<ILoggingService> LoggingServiceMock { get; set; }

        protected List<Log> ApiLogs { get; set; }

        protected List<Log> IdentityLogs { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);
            DbContextMock = MockRepository.Create<EdFiLogDbContext>();
            DbContextFactoryMock = MockRepository.Create<IDbContextFactory<EdFiLogDbContext>>();
            LoggingServiceMock = MockRepository.Create<ILoggingService>();
        }

        [SetUp]
        public void SetUp()
        {
            var logs = new List<Log>();

            IdentityLogs = new List<Log>();
            for (var i = 0; i < 10; i++)
            {
                IdentityLogs.Add(
                    new Log
                    {
                        Id = i,
                        Url = new Uri($"https://test.wearedoubleline.com/{EdFiApiLogService.ApiName}/identity/").ToString(),
                        Method = $"method {i}"
                    });
            }

            logs.AddRange(IdentityLogs);

            ApiLogs = new List<Log>();
            for (var i = 10; i < 20; i++)
            {
                ApiLogs.Add(
                    new Log
                    {
                        Id = i,
                        Url = new Uri($"https://test.wearedoubleline.com/{EdFiApiLogService.ApiName}/data/v3/").ToString(),
                        Method = $"method {i}"
                    });
            }

            logs.AddRange(ApiLogs);

            EntityFrameworkMocks.SetupMockDbSet(
                EntityFrameworkMocks.GetQueryableMockDbSet(logs),
                DbContextMock,
                x => x.Logs,
                x => x.Logs = It.IsAny<DbSet<Log>>(),
                logs);

            DbContextMock.As<IDisposable>().Setup(x => x.Dispose());
            DbContextFactoryMock.Setup(x => x.Create()).Returns(DbContextMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            DbContextFactoryMock.Reset();
            LoggingServiceMock.Reset();
        }
        
        [Test]
        public void GetIdentityIssues_Should_ReturnFilteredIssues()
        {
           var edfiApiLogService = new EdFiApiLogService(
                LoggingServiceMock.Object,
                DbContextFactoryMock.Object);

            var result = edfiApiLogService.GetIdentityIssues();
            
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.ShouldHaveSameItems(IdentityLogs);
        }

        [Test]
        public void GetApiIssues_Should_ReturnFilteredIssues()
        {
            var edfiApiLogService = new EdFiApiLogService(
                LoggingServiceMock.Object,
                DbContextFactoryMock.Object);

            var result = edfiApiLogService.GetApiErrors();
            
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.ShouldHaveSameItems(ApiLogs);
        }
    }
}
