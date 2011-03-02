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

        private readonly string _websiteRootDirectory;
        private VirtualPathResolver _resolver;
        private System.Configuration.Configuration _config;
    	private ResourceManagementContext _context;
        private ILogger _logger = NullLogger.Instance;
    	
        private PreConsolidateCommand(string websiteRootDirectory)
        {
            _websiteRootDirectory = websiteRootDirectory;
			Mode = ResourceMode.Release;
        }

        /// <summary>
        /// Gets the physical path the the root of the website.
        /// </summary>
        public string WebsiteRootDirectory
        {
            get { return _websiteRootDirectory; }
        }

		/// <summary>
		/// Indicates whether to consolidate the scripts in Debug or Release mode.
		/// </summary>
		public ResourceMode Mode { get; set; }

		/// <summary>
		/// Optionally applies the given version to be used in all script/style includes.  If left null,
		/// the version will be generated based on the Date/Time the scripts and styles were consolidated.
		/// </summary>
		public string Version { get; set; }

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
            ResourceManagementConfiguration configSection = GetConfigSection(WebsiteRootDirectory);
            _resolver = GetResolver(configSection.RootFilePath);
			
			SetCompression(configSection);
			ConsolidateAll(configSection);
			SetVersion(configSection); 
			
			SaveConfigChanges(configSection);
            LogMessage("End consolidating resources.");
        }

		private void SetCompression(ResourceManagementConfiguration configSection)
		{
			var compress = Mode == ResourceMode.Release;
			configSection.ClientScripts.Compress = compress;
			configSection.CssFiles.Compress = compress;
		}

    	private void SetVersion(ResourceManagementConfiguration section)
    	{
    		section.Version = this.Version ?? DateTime.Now.ToString("yyMMddHHmm");
		}

		private void ConsolidateAll(ResourceManagementConfiguration configSection)
		{
			_context = configSection.BuildContext();
			_context.ConsolidateAll(WriteConsolidatedResource, Mode);
			//TODO: Persist the pre-consolidation report
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
            return VirtualPathResolver.GetInstance(webRoot);
        }
    }
}