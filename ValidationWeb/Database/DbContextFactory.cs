using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ValidationWeb.Database
{
    public class DbContextFactory<T> : IDbContextFactory<T>
        where T : DbContext, new()
    {
        public T Create()
        {
            return new T();
        }
    }
}