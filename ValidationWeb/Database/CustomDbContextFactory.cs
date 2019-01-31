namespace ValidationWeb.Database
{
    using System;
    using System.Data.Entity;
    
    public class CustomDbContextFactory<T> : ICustomDbContextFactory<T>
        where T : DbContext, new()
    {
        public T Create(string customParam)
        {
            return Activator.CreateInstance(typeof(T), customParam) as T;
        }
    }
}
