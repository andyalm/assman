using System;

namespace Assman.Configuration
{
    public interface IConfigLoader
    {
        TSection GetSection<TSection>(string sectionName) where TSection : class;
    }
}