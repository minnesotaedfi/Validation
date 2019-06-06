namespace ValidationWeb.Database
{
    public interface ISchoolYearDbContextFactory
    {
        RawOdsDbContext CreateWithParameter(string schoolYear);
    }
}