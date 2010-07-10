using System;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.ContentFiltering;

namespace AlmWitt.Web.ResourceManagement
{
    public class DefaultContentFilterFactory : IContentFilterFactory
    {
        public IContentFilter CreateClientScriptFilter(ClientScriptGroupElement groupElement)
        {
            if (groupElement.Compress)
            {
                return new JSMinFilter();
            }
            else
            {
                return null;
            }
        }

        public IContentFilter CreateCssFilter(CssGroupElement groupElement)
        {
            return null;
        }
    }
}