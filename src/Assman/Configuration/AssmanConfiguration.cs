using System;
using System.Configuration;
using System.Linq;
using System.Web;

using Assman.ContentFiltering;
using Assman.PreCompilation;

namespace Assman.Configuration
{
	/// <summary>
	/// Represents the configuration section for resource management.
	/// </summary>
	public class AssmanConfiguration : ConfigurationSection
	{
		private const string SectionName = "assman";
		private string _rootFilePath;
		private static readonly AssmanConfiguration _defaultSection = new AssmanConfiguration();
		private static AssmanConfiguration _config;
		private static IConfigLoader _configLoader = new DefaultConfigLoader();

		/// <summary>
		/// Gets the current configuration.
		/// </summary>
		public static AssmanConfiguration Current
		{
			get
			{
				if(_config == null)
				{
					_config = _configLoader.GetSection<AssmanConfiguration>(SectionName) ?? _defaultSection;
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

		private VirtualPathResolver _pathResolver;

		private VirtualPathResolver PathResolver
		{
			get
			{
				if(_pathResolver == null)
				{
					_pathResolver = VirtualPathResolver.GetInstance(RootFilePath);
				}

				return _pathResolver;
			}
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
					var httpContext = HttpContext.Current;
					if(httpContext != null)
						_rootFilePath = httpContext.Server.MapPath("~");
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
		/// Gets or sets whether consolidation should be enabled.
		/// </summary>
		[ConfigurationProperty(PropertyNames.GZip, DefaultValue = false)]
		public bool GZip
		{
			get { return (bool)this[PropertyNames.GZip]; }
			set { this[PropertyNames.GZip] = value; }
		}

		/// <summary>
		/// Gets or sets whether dependencies provided by the <see cref="IDependencyProvider"/>'s will be included automatically.
		/// </summary>
		[ConfigurationProperty(PropertyNames.ManageDependencies, DefaultValue = true)]
		public bool ManageDependencies
		{
			get { return (bool)this[PropertyNames.ManageDependencies]; }
			set { this[PropertyNames.ManageDependencies] = value; }
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
		/// Gets the <see cref="ScriptGroupElement"/> used to configure client script resources.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Scripts, IsRequired = false)]
		public ScriptsConfigurationElement Scripts
		{
			get
			{
				var element = this[PropertyNames.Scripts] as ScriptsConfigurationElement;

				if(element == null)
				{
					element = new ScriptsConfigurationElement();
					this[PropertyNames.Scripts] = element;
				}
				return element;
			}
		}

		/// <summary>
		/// Gets the <see cref="StylesheetGroupElement"/> used to configure css resources.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Stylesheets, IsRequired = false)]
		public StylesheetsConfigurationElement Stylesheets
		{
			get
			{
				var element = this[PropertyNames.Stylesheets] as StylesheetsConfigurationElement;

				if (element == null)
				{
					element = new StylesheetsConfigurationElement();
					this[PropertyNames.Stylesheets] = element;
				}
				return element;
			}
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
		public DateTime LastModified(VirtualPathResolver pathResolver)
		{
			return _lastModified ?? ConfigurationHelper.LastModified(this, pathResolver);
		}

		public void LastModified(DateTime value)
		{
			_lastModified = value;
		}

		public AssmanContext BuildContext(bool usePreCompilationReportIfPresent = true)
		{
			IResourceFinder fileFinder = ResourceFinderFactory.Null;
			IPreCompiledReportPersister preCompiledPersister = NullPreCompiledPersister.Instance;
			
			if(!String.IsNullOrEmpty(RootFilePath))
			{
				fileFinder = ResourceFinderFactory.GetInstance(RootFilePath);
				if(usePreCompilationReportIfPresent)
					preCompiledPersister = CompiledFilePersister.ForWebDirectory(RootFilePath);
			}

			return BuildContext(fileFinder, preCompiledPersister);
		}

		public AssmanContext BuildContext(IResourceFinder fileFinder, IPreCompiledReportPersister preCompiledPersister)
		{
		    var resourceMode = ResourceModeProvider.Instance.GetCurrentResourceMode();
		    var context = AssmanContext.Create(resourceMode);

			context.ConfigurationLastModified = LastModified(PathResolver);
			context.ConsolidateScripts = Consolidate && Scripts.Groups.Consolidate;
			context.ConsolidateStylesheets = Consolidate && Stylesheets.Groups.Consolidate;
			context.GZip = GZip;
			context.ManageDependencies = ManageDependencies;
			context.AddFinder(fileFinder);
			context.AddAssemblies(Assemblies.GetAssemblies());
			context.ScriptGroups.AddGlobalDependencies(Scripts.GlobalDependencies.Cast<GlobalDependenciesElement>().Select(e => e.Path));
			context.ScriptGroups.AddRange(Scripts.Groups.Cast<IResourceGroupTemplate>());
			context.StyleGroups.AddGlobalDependencies(Stylesheets.GlobalDependencies.Cast<GlobalDependenciesElement>().Select(e => e.Path));
			context.StyleGroups.AddRange(Stylesheets.Groups.Cast<IResourceGroupTemplate>());
			context.MapExtensionToContentPipeline(".js", DefaultPipelines.Javascript());
			context.MapExtensionToContentPipeline(".css", DefaultPipelines.Css());
			context.MapExtensionToDependencyProvider(".js", VisualStudioScriptDependencyProvider.GetInstance());
			context.MapExtensionToDependencyProvider(".css", CssDependencyProvider.GetInstance());

			PreCompilationReport preCompilationReport;
			if (preCompiledPersister.TryGetPreConsolidationInfo(out preCompilationReport))
			{
				context.LoadPreCompilationReport(preCompilationReport);
			}

			foreach (var plugin in Plugins.GetPlugins())
			{
				plugin.Initialize(context);
			}
			
			return context;
		}

		public void SavePreConsolidationReport(PreCompilationReport report)
		{
			var preConsolidationPersister = CompiledFilePersister.ForWebDirectory(RootFilePath);
			preConsolidationPersister.SavePreConsolidationInfo(report);
		}
	}
}
