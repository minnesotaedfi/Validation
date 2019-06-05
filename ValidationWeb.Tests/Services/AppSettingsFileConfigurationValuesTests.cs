using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;
using Should;
using Should.Extensions;
using ValidationWeb.Models;
using ValidationWeb.Services.Implementations;

namespace ValidationWeb.Tests.Services
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "tests don't follow standard naming")]
    public class AppSettingsFileConfigurationValuesTests
    {
        protected MockRepository MockRepository { get; set; }

        protected AppSettingsFileConfigurationValues AppSettingsFileConfigurationValues { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);
        } 

        [SetUp]
        public void SetUp()
        {
            AppSettingsFileConfigurationValues = new AppSettingsFileConfigurationValues();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void AppId_Should_ReturnSetValue()
        {
            VerifyConfigProperty(
                "SsoAppId", 
                AppSettingsFileConfigurationValues, 
                x => x.AppId, 
                "test_SsoAppId");
        }

        [Test]
        public void AuthenticationServerRedirectUrl_Should_ReturnSetValue()
        {
            VerifyConfigProperty(
                "AuthenticationServerRedirectUrl", 
                AppSettingsFileConfigurationValues, 
                x => x.AuthenticationServerRedirectUrl, 
                "test_AuthenticationServerRedirectUrl");
        }
        
        [Test]
        public void AuthorizationStoredProcedureName_Should_ReturnSetValue()
        {
            VerifyConfigProperty(
                "AuthorizationStoredProcedureName", 
                AppSettingsFileConfigurationValues, 
                x => x.AuthorizationStoredProcedureName, 
                "test_AuthorizationStoredProcedureName");
        }

        [Test]
        public void SeedSchoolYears_Should_Parse()
        {
            // in app.config: "2018,2019,2020"
            var expectedResult = new List<SchoolYear>(new []
            {
                new SchoolYear{ StartYear = "2017", EndYear = "2018" },
                new SchoolYear{ StartYear = "2018", EndYear = "2019" },
                new SchoolYear{ StartYear = "2019", EndYear = "2020" },
            });

            var result = AppSettingsFileConfigurationValues.SeedSchoolYears;

            result.ShouldHaveSameItems(
                expectedResult, 
                (x, y) => x.StartYear == y.StartYear && x.EndYear == y.EndYear);
        }

        private void VerifyConfigProperty<T>(
            string settingName, 
            AppSettingsFileConfigurationValues settings, 
            Func<AppSettingsFileConfigurationValues, T> expr, 
            T testValue)
        {
            var result = expr.Invoke(settings);
            result.ShouldEqual(testValue);
        }
    }
}
