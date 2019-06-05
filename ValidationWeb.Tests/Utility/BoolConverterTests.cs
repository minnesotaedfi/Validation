using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Should;
using ValidationWeb.Utility;

namespace ValidationWeb.Tests.Utility
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class BoolConverterTests
    {
        static BoolConverterTests()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);
        }

        protected static MockRepository MockRepository { get; set; }

        [Test]
        [TestCase(true, TestName = "WriteJson_True")]
        [TestCase(false, TestName = "WriteJson_False")]
        public void WriteJson_Should_WriteBooleanValue(bool testValue)
        {
            int? writtenResult = null;

            var jsonWriterMock = MockRepository.Create<JsonWriter>();

            jsonWriterMock
                .Setup(x => x
                .WriteValue(It.IsAny<int>()))
                .Callback<int>((val) => writtenResult = val);

            var boolConverter = new BoolConverter();

            boolConverter.WriteJson(jsonWriterMock.Object, testValue, null);

            jsonWriterMock.Verify(x => x.WriteValue(It.IsAny<int>()), Times.Once);

            writtenResult.ShouldEqual(testValue ? 1 : 0);
        }

        [Test]
        [TestCase("1", true, TestName = "ReadJson_True")]
        [TestCase("anything but 1", false, TestName = "ReadJson_Untrue")]
        [TestCase("", false, TestName = "ReadJson_Empty")]
        [TestCase(null, false, TestName = "ReadJson_Null")]
        public void ReadJson_Should_ReadBooleanValue(string testValue, bool expectedResult)
        {
            var boolConverter = new BoolConverter();
         
            var jsonReaderMock = MockRepository.Create<JsonReader>();
            jsonReaderMock.Setup(x => x.Value).Returns(testValue);

            var result = boolConverter.ReadJson(
                jsonReaderMock.Object, 
                typeof(bool),
                null, 
                null);

            jsonReaderMock.VerifyGet(x => x.Value, Times.Once);
            result.ShouldBeType<bool>();
            result.ShouldEqual(expectedResult);
        }

        [Test]
        [TestCase(typeof(bool), true, TestName = "CanConvert_bool")]
        [TestCase(typeof(float), false, TestName = "CanConvert_random")]
        public void CanConvert_Should_RecognizeBooleanType(Type testType, bool expectedResult)
        {
            var boolConverter = new BoolConverter();
            var result = boolConverter.CanConvert(testType);
            result.ShouldEqual(expectedResult);
        }
    }
}
