using System;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace Assman.Configuration
{
    public class ScriptsConfigurationElement : ResourceConfigurationElement<ScriptGroupElementCollection>
    {
        [ConfigurationProperty(PropertyNames.JsCompressionOverride, IsRequired = false)]
        public CultureConfigurationElement JsCompressionOverride
        {
            get { return (this[PropertyNames.JsCompressionOverride] as CultureConfigurationElement) ?? CultureConfigurationElement.Default; }
            set { this[PropertyNames.JsCompressionOverride] = value; }
        }
    }

    public class CultureConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty(PropertyNames.Culture, IsRequired=true, DefaultValue = "")]
        public CultureInfo Culture
        {
            get
            {
                var culture = this[PropertyNames.Culture] as CultureInfo;
                if (culture != null && CultureInfo.GetCultures(CultureTypes.AllCultures).Any(ci => ci.Equals(culture)))
                    return culture;
                return null;
            }
            set { this[PropertyNames.Culture] = value; }
        }

        public static CultureConfigurationElement Default { get { return new CultureConfigurationElement { Culture = null }; } }
    }
}