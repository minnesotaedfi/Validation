using System.Data.Entity;

namespace ValidationWeb.Database
{
    public interface IDbContextFactoryWithParam<out T>
        where T : DbContext, new()
    {
        // U GetNewDbContext();
        T Create();

        //T Create(string fourDigitSchoolYear);
    }
}