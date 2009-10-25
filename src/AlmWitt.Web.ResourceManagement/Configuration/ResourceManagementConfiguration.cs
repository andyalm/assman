using System;
using System.Configuration;
using System.Reflection;
using System.Web.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	/// <summary>
	/// Represents the configuration section for resource management.
	/// </summary>
	public class ResourceManagementConfiguration : ConfigurationSection
	{
		private const string SECTION_NAME = "almWitt.web.resourceManagement";
		private static readonly ResourceManagementConfiguration _defaultSection = new ResourceManagementConfiguration();
		private static ResourceManagementConfiguration _config = null;

		/// <summary>
		/// Gets the current configuration.
		/// </summary>
		public static ResourceManagementConfiguration Current
		{
			get
			{
				if(_config == null)
				{
					_config = WebConfigurationManager.GetSection(SECTION_NAME) as ResourceManagementConfiguration;
					if (_config == null)
						_config = _defaultSection;
				}
				

				return _config;
			}
			set
			{
				_config = value;
			}
		}
		
		/// <summary>
		/// Opens an instance of the configuration to be edited.
		/// </summary>
		/// <param name="configFilePath">The physical path to the config file.</param>
		/// <param name="configuration">The <see cref="Configuration"/> object used to manage the configuration.</param>
		/// <returns></returns>
		public static ResourceManagementConfiguration OpenForEditing(string configFilePath, out System.Configuration.Configuration configuration)
		{
			configuration = ConfigurationHelper.OpenWebConfiguration(configFilePath);

			return ConfigurationHelper.OpenForEditing<ResourceManagementConfiguration>(SECTION_NAME, configuration);
		}

		/// <summary>
		/// Gets or sets whether consolidation should be enabled.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Consolidate, DefaultValue = false)]
		public bool Consolidate
		{
			get { return (bool) this[PropertyNames.Consolidate]; }
			set { this[PropertyNames.Consolidate] = value; }
		}

		/// <summary>
		/// Gets or sets whether the resources have been pre-consolidated into static files.
		/// </summary>
		[ConfigurationProperty(PropertyNames.PreConsolidated)]
		public bool PreConsolidated
		{
			get { return (bool) this[PropertyNames.PreConsolidated]; }
			set { this[PropertyNames.PreConsolidated] = value; }
		}

		/// <summary>
		/// Gets the <see cref="ClientScriptGroupElement"/> used to configure client script resources.
		/// </summary>
		[ConfigurationProperty(PropertyNames.ClientScripts, IsRequired = false)]
		[ConfigurationCollection(typeof(ClientScriptGroupElementCollection), AddItemName = "group")]
		public ClientScriptGroupElementCollection ClientScripts
		{
			get { return this[PropertyNames.ClientScripts] as ClientScriptGroupElementCollection; }
		}

		/// <summary>
		/// Gets the <see cref="CssGroupElement"/> used to configure css resources.
		/// </summary>
		[ConfigurationProperty(PropertyNames.CssFiles, IsRequired = false)]
		[ConfigurationCollection(typeof(ResourceGroupElementCollection<ClientScriptGroupElement>), AddItemName = "group")]
		public CssGroupElementCollection CssFiles
		{
			get { return this[PropertyNames.CssFiles] as CssGroupElementCollection; }
		}

	    /// <summary>
	    /// Gets the assemblies that the library will search through for embedded resources.
	    /// </summary>
        [ConfigurationProperty(PropertyNames.Assemblies, IsRequired = false)]
	    [ConfigurationCollection(typeof(AssemblyElementCollection), AddItemName = "add", RemoveItemName = "remove", ClearItemsName = "clear")]
	    public AssemblyElementCollection Assemblies
	    {
            get
            {
                AssemblyElementCollection assemblies = this[PropertyNames.Assemblies] as AssemblyElementCollection;
                if(assemblies == null)
                {
                    assemblies = new AssemblyElementCollection();
                    this[PropertyNames.Assemblies] = assemblies;
                }

                return assemblies;
            }
	    }

        [ConfigurationProperty(PropertyNames.CustomResourceFinders, IsRequired = false)]
        [ConfigurationCollection(typeof(CustomFinderElementCollection), AddItemName = "add", RemoveItemName = "remove", ClearItemsName = "clear")]
        public CustomFinderElementCollection CustomResourceFinders
        {
            get
            {
                var finders = this[PropertyNames.CustomResourceFinders] as CustomFinderElementCollection;
                if (finders == null)
                {
                    finders = new CustomFinderElementCollection();
                    this[PropertyNames.CustomResourceFinders] = finders;
                }

                return finders;
            }
        }
        
        /// <summary>
        /// Adds the configured assemblies to the finder so that embedded resources will be found in those assemblies.
        /// </summary>
        /// <param name="finder"></param>
        /// <returns></returns>
        public IResourceFinder AddCustomFinders(IResourceFinder finder)
        {
            CompositeResourceFinder compositeFinder = new CompositeResourceFinder();
            compositeFinder.AddFinder(finder);

            foreach(Assembly assembly in Assemblies.GetAssemblies())
            {
                compositeFinder.AddFinder(ResourceFinderFactory.GetInstance(assembly));
            }

            compositeFinder.AddFinders(CustomResourceFinders.GetFinders());

            return compositeFinder;
        }

		/// <summary>
		/// Saves the configuration.
		/// </summary>
		/// <param name="config"></param>
		public void SaveChanges(System.Configuration.Configuration config)
		{
			config.Save();
		}

		internal string GetScriptUrl(string scriptUrl)
		{
			return GetResourceUrl(ClientScripts, scriptUrl);
		}

		internal string GetStylesheetUrl(string stylesheetUrl)
		{
			return GetResourceUrl(CssFiles, stylesheetUrl);
		}

		private string GetResourceUrl<TGroupElement>(ResourceGroupElementCollection<TGroupElement> groupElements, string resourceUrl) where TGroupElement : ResourceGroupElement, new()
		{
			if (Consolidate && groupElements.Consolidate)
			{
				foreach (ResourceGroupElement groupElement in groupElements)
				{
					if(groupElement.IsMatch(resourceUrl))
						return groupElement.GetResourceUrl(resourceUrl, ConsolidatedUrlType);
				}
			}
				
			return resourceUrl;
		}

		private UrlType ConsolidatedUrlType
		{
			get
			{
				return PreConsolidated ? UrlType.Static : UrlType.Dynamic;
			}
		}

		private static class PropertyNames
		{
			public const string Consolidate = "consolidate";
			public const string ClientScripts = "clientScripts";
			public const string CssFiles = "cssFiles";
			public const string PreConsolidated = "preConsolidated";
		    public const string Assemblies = "assemblies";
            public const string CustomResourceFinders = "customResourceFinders";
		}
	}
}
