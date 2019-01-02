namespace ValidationWeb.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    using Moq;

    public class EntityFrameworkMocks
    {
        static EntityFrameworkMocks()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);
        }
        
        protected static MockRepository MockRepository { get; set; }
        
        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = MockRepository.Create<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(sourceList.Add);

            return dbSet;
        }

        public static void SetupMockDbSet<T, U>(
            Mock<DbSet<U>> queryableMockDbSet,
            Mock<T> dbContextMock,
            Expression<Func<T, DbSet<U>>> setupExpression,
            Action<T> setterExpression,
            List<U> source)
            where T : DbContext
            where U : class
        {
            dbContextMock.Setup(x => x.Set<U>()).Returns(queryableMockDbSet.Object);
            dbContextMock.Setup(setupExpression).Returns(() => queryableMockDbSet.Object);
            dbContextMock.Setup(x => x.SaveChanges()).Returns(1);
            dbContextMock.SetupSet(setterExpression);
        }
    }
}