using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.ContentFiltering;

namespace AlmWitt.Web.ResourceManagement
{
    public interface IContentFilterFactory
    {
        IContentFilter CreateClientScriptFilter(ClientScriptGroupElement groupElement);
        IContentFilter CreateCssFilter(CssGroupElement groupElement);
    }
}