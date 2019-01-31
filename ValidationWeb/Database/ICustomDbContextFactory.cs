namespace ValidationWeb.Database
{
    using System.Data.Entity;

    public interface ICustomDbContextFactory<out T>
        where T : DbContext, new()
    {
        T Create(string customParam);
    }
}