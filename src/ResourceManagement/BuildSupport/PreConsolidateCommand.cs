using System;

using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement.BuildSupport
{
    /// <summary>
    /// Consolidates javascript and css resources into fewer files according to the
    /// resource management configuration in the web.config.
    /// </summary>
    public class PreConsolidateCommand
    {
        /// <summary>
        /// Gets an instance of a <see cref="PreConsolidateCommand"/>.
        /// </summary>
        /// <param name="webRoot">The full physical path to the the root of the website.</param>
        /// <returns></returns>
        public static PreConsolidateCommand GetInstance(string webRoot)
        {
            return new PreConsolidateCommand(webRoot);
        }

        private readonly string _webRoot;
        private VirtualPathResolver _resolver;
        private System.Configuration.Configuration _config;
    	private ResourceManagementContext _context;
        private ILogger _logger = NullLogger.Instance;
    	private bool? _enableCompression;

        private PreConsolidateCommand(string webRoot)
        {
            _webRoot = webRoot;
        }

        /// <summary>
        /// Gets or sets the physical path the the root of the website.
        /// </summary>
        public string WebRoot
        {
            get { return _webRoot; }
        }

    	public bool AutoVersion { get; set; }

    	public bool EnableCompression
    	{
			get { return _enableCompression ?? false; }
			set { _enableCompression = value; }
    	}

        /// <summary>
        /// Gets or sets the build logger used to log the progress of the command.
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns></returns>
        public void Execute()
        {
            LogMessage("Begin consolidating resources...");
            ResourceManagementConfiguration configSection = GetConfigSection(WebRoot);
            _resolver = GetResolver(configSection.RootFilePath);
			
			SetCompression(configSection);
			ConsolidateAll(configSection);
			SetVersion(configSection); 
			
			SaveConfigChanges(configSection);
            LogMessage("End consolidating resources.");
        }

		private void SetCompression(ResourceManagementConfiguration configSection)
		{
			if (_enableCompression != null)
			{
				configSection.ClientScripts.Compress = _enableCompression.Value;
				configSection.CssFiles.Compress = _enableCompression.Value;
			}
		}

		private void SetVersion(ResourceManagementConfiguration section)
		{
			if (AutoVersion)
			{
				string version = DateTime.Now.ToString("MMdd");
				section.Version = version;
			}
		}

		private void ConsolidateAll(ResourceManagementConfiguration configSection)
		{
			_context = configSection.BuildContext();
			_context.ConsolidateAll(WriteConsolidatedResource);
			configSection.PreConsolidated = true;
		}

		private void WriteConsolidatedResource(ConsolidatedResource consolidatedResource, IResourceGroup group)
		{
			string consolidatedPath = _resolver.MapPath(group.ConsolidatedUrl);
			consolidatedResource.WriteToFile(consolidatedPath);
			LogConsolidation(consolidatedResource, group.ConsolidatedUrl);
		}

        private void LogConsolidation(ConsolidatedResource consolidatedResource, string consolidatedUrl)
        {
            LogMessage(String.Format("Consolidating '{0}'...", consolidatedUrl));
            foreach (IResource resource in consolidatedResource.Resources)
            {
                LogMessage(String.Format("\t...from '{0}'", resource.VirtualPath));
            }
        }

        private void LogMessage(string message)
        {
            _logger.LogMessage(message);
        }

        private ResourceManagementConfiguration GetConfigSection(string webRoot)
        {
            ResourceManagementConfiguration.ConfigLoader = new MappedConfigLoader(webRoot);

            ResourceManagementConfiguration configSection = ResourceManagementConfiguration.OpenForEditing(out _config);
            configSection.RootFilePath = webRoot;
            //set this instance of the config section to current so that the components
            //that need to read from configuration can see it.
            ResourceManagementConfiguration.Current = configSection;

            return configSection;
        }

        private void SaveConfigChanges(ResourceManagementConfiguration configSection)
        {
            configSection.SaveChanges(_config);
        }

        private VirtualPathResolver GetResolver(string webRoot)
        {
            return new VirtualPathResolver(webRoot);
        }
    }
}