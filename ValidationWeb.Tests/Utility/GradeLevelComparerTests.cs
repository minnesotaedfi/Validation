namespace ValidationWeb.Tests.Utility
{
    using System.Diagnostics.CodeAnalysis;

    using NUnit.Framework;

    using Should;

    using ValidationWeb.Utility;

    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class GradeLevelComparerTests
    {
        [Test]
        public void Compare_Should_SeeLeftIntLessThanRightInt()
        {
            var left = "2";
            var right = "3"; 
            var comparer = new GradeLevelComparer();

            var result = comparer.Compare(left, right);

            result.ShouldBeLessThan(0);
        }

        [Test]
        public void Compare_Should_SeeLeftIntEqualsRightInt()
        {
            var left = "3";
            var right = "3"; 
            var comparer = new GradeLevelComparer();

            var result = comparer.Compare(left, right);

            result.ShouldEqual(0);
        }

        [Test]
        public void Compare_Should_SeeLeftIntGreaterThanRightInt()
        {
            var left = "5";
            var right = "3"; 
            var comparer = new GradeLevelComparer();

            var result = comparer.Compare(left, right);

            result.ShouldBeGreaterThan(0);
        }

        [Test]
        public void Compare_Should_SeeLeftStringGreaterThanRightString()
        {
            var left = "aaa";
            var right = "bbb"; 
            var comparer = new GradeLevelComparer();

            var result = comparer.Compare(left, right);

            result.ShouldBeLessThan(0);
        }

        [Test]
        public void Compare_Should_SeeLeftStringEqualsRightString()
        {
            var left = "aaa";
            var right = "aaa"; 
            var comparer = new GradeLevelComparer();

            var result = comparer.Compare(left, right);

            result.ShouldEqual(0);
        }

        [Test]
        public void Compare_Should_SeeLeftStringLessThanRightString()
        {
            var left = "bbb";
            var right = "aaa"; 
            var comparer = new GradeLevelComparer();

            var result = comparer.Compare(left, right);

            result.ShouldBeGreaterThan(0);
        }

        [Test]
        public void Compare_Should_SeeLeftStringLessThanRightInt()
        {
            var left = "aaa";
            var right = "123"; 
            var comparer = new GradeLevelComparer();

            var result = comparer.Compare(left, right);

            result.ShouldBeLessThan(0);
        }

        [Test]
        public void Compare_Should_SeeLeftIntGreaterThanRightString()
        {
            var left = "123";
            var right = "aaa"; 
            var comparer = new GradeLevelComparer();

            var result = comparer.Compare(left, right);

            result.ShouldBeGreaterThan(0);
        }
    }
}
