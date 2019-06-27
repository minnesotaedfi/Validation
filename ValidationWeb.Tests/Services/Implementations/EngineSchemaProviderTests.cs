using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;
using Should;
using ValidationWeb.Services.Implementations;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Tests.Services.Implementations
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class EngineSchemaProviderTests
    {
        protected MockRepository MockRepository { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);
        }

        [Test]
        public void EngineSchemaProvider_Should_StoreValue()
        {
            var testValue = "test test test"; 

            var rulesConfigurationValuesMock = MockRepository.Create<IRulesEngineConfigurationValues>();
            rulesConfigurationValuesMock.SetupGet(x => x.RuleEngineResultsSchema).Returns(testValue);

            var engineSchemaProvider = new EngineSchemaProvider(rulesConfigurationValuesMock.Object);

            var result = engineSchemaProvider.Value;

            result.ShouldEqual(testValue);
        }
    }
}
