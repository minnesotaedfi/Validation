namespace ValidationWeb.Database
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public class DbContextFactory<T> : IDbContextFactory<T>
        where T : DbContext, new()
    {
        public T Create()
        {
            return new T();
        }
    }
}
