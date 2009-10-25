using System.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
    public interface IConfigLoader
    {
        TSection GetSection<TSection>(string sectionName) where TSection : class;

        TSection GetSectionForEditing<TSection>(string sectionName, out System.Configuration.Configuration configuration) where TSection : ConfigurationSection, new();
    }
}