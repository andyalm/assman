namespace AlmWitt.Web.ResourceManagement.Configuration
{
    public interface IConfigLoader
    {
        TSection GetSection<TSection>(string sectionName) where TSection : class;
    }
}