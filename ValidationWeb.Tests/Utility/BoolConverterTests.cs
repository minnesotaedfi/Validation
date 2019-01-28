namespace ValidationWeb.Tests
{

    using Moq;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using Should;

    [TestFixture]
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
    }
}
