using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

using AlmWitt.Web.ResourceManagement.ContentFiltering;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	/// <summary>
	/// Represents the configuration section for resource management.
	/// </summary>
	public class ResourceManagementConfiguration : ConfigurationSection
	{
		private const string SECTION_NAME = "almWitt.web.resourceManagement";
        private string _rootFilePath;
		private static readonly ResourceManagementConfiguration _defaultSection = new ResourceManagementConfiguration();
		private static ResourceManagementConfiguration _config = null;
	    private static IConfigLoader _configLoader = new DefaultConfigLoader();

		/// <summary>
		/// Gets the current configuration.
		/// </summary>
		public static ResourceManagementConfiguration Current
		{
			get
			{
				if(_config == null)
				{
				    _config = _configLoader.GetSection<ResourceManagementConfiguration>(SECTION_NAME) ?? _defaultSection;
				}

				return _config;
			}
			set
			{
				_config = value;
			}
		}

	    /// <summary>
	    /// Gets or sets the <see cref="IConfigLoader"/> that is used by the resource consolidation framework
	    /// to load config settings.
	    /// </summary>
        public static IConfigLoader ConfigLoader
	    {
	        get
	        {
	            return _configLoader;
	        }
            set
            {
                _configLoader = value;
            }
	    }
		
		/// <summary>
		/// Opens an instance of the configuration to be edited.
		/// </summary>
		/// <param name="configFilePath">The physical path to the config file.</param>
		/// <param name="configuration">The <see cref="Configuration"/> object used to manage the configuration.</param>
		/// <returns></returns>
		public static ResourceManagementConfiguration OpenForEditing(out System.Configuration.Configuration configuration)
		{
		    return _configLoader.GetSectionForEditing<ResourceManagementConfiguration>(SECTION_NAME, out configuration);
		}

        /// <summary>
        /// Gets the full filesytem path to the root of the website.
        /// </summary>
        public string RootFilePath
        {
            get
            {
                if (_rootFilePath == null)
                {
                    _rootFilePath = HttpContext.Current.Server.MapPath("~");
                }

                return _rootFilePath;
            }
            set
            {
                _rootFilePath = value;
            }
        }

		/// <summary>
		/// Gets or sets whether consolidation should be enabled.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Consolidate, DefaultValue = true)]
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
		/// Gets or sets the version number that will be appended to the end of all resource url's.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Version)]
		public string Version
		{
			get { return (string)this[PropertyNames.Version]; }
			set { this[PropertyNames.Version] = value; }
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

		[ConfigurationProperty(PropertyNames.Plugins, IsRequired = false)]
		[ConfigurationCollection(typeof(PluginElementCollection), AddItemName = "add", RemoveItemName = "remove", ClearItemsName = "clear")]
		public PluginElementCollection Plugins
		{
			get
			{
				var finders = this[PropertyNames.Plugins] as PluginElementCollection;
				if(finders == null)
				{
					finders = new PluginElementCollection();
					this[PropertyNames.Plugins] = finders;
				}

				return finders;
			}
		}

		private DateTime? _lastModified;
		public DateTime LastModified()
		{
			return _lastModified ?? ConfigurationHelper.GetLastModified(this);
		}

		public void LastModified(DateTime value)
		{
			_lastModified = value;
		}

		public ResourceManagementContext BuildContext()
		{
			return BuildContext(ResourceFinderFactory.GetInstance(RootFilePath));
		}

		public ResourceManagementContext BuildContext(IResourceFinder fileFinder)
		{
			var context = ResourceManagementContext.Create();

			context.AddFinder(fileFinder);
			context.AddAssemblies(Assemblies.GetAssemblies());
			context.ClientScriptGroups.AddRange(ClientScripts.Cast<IResourceGroupTemplate>());
			context.CssFileGroups.AddRange(CssFiles.Cast<IResourceGroupTemplate>());
			context.MapExtensionToFilter(".js", JSMinContentFilterFactory.GetInstance());

			foreach (var plugin in Plugins.GetPlugins())
			{
				plugin.Initialize(context);
			}
			
			return context;
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
					{
						resourceUrl = groupElement.GetResourceUrl(resourceUrl, ConsolidatedUrlType);
						break;
					}
				}
			}
			if (!String.IsNullOrEmpty(Version) && !resourceUrl.Contains("?"))
				resourceUrl += "?v=" + HttpUtility.UrlEncode(Version);	
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
			public const string Version = "version";
			public const string Assemblies = "assemblies";
			public const string Plugins = "plugins";
		}
	}
}
