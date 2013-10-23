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
            get{return (this[PropertyNames.JsCompressionOverride] as CultureConfigurationElement) ?? CultureConfigurationElement.Default;}
            set { this[PropertyNames.JsCompressionOverride] = value; }
        }
    }

    public class CultureConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty(PropertyNames.Culture, DefaultValue = "")]
        public CultureInfo Culture
        {
            get
            {
                var cultureName = this[PropertyNames.JsCompressionOverride] as string;
                if (cultureName != null && CultureInfo.GetCultures(CultureTypes.AllCultures).Any(ci => ci.Name == cultureName))
                    return new CultureInfo(cultureName);
                return null;
            }
            set { this[PropertyNames.JsCompressionOverride] = value; }
        }

        public static CultureConfigurationElement Default {get {return new CultureConfigurationElement{Culture = null};}}
    }
}