using System;
using System.IO;
using System.Reflection;

using Assman.Configuration;

namespace Assman.BuildSupport
{
	/// <summary>
	/// Consolidates javascript and css resources into fewer files according to the
	/// resource management configuration in the web.config.
	/// </summary>
	public class PreCompileCommand
	{
		/// <summary>
		/// Gets an instance of a <see cref="PreCompileCommand"/>.
		/// </summary>
		/// <param name="webRoot">The full physical path to the the root of the website.</param>
		/// <returns></returns>
		public static PreCompileCommand GetInstance(string webRoot)
		{
			return new PreCompileCommand(webRoot);
		}

		private readonly string _websiteRootDirectory;
		private ILogger _logger = NullLogger.Instance;
		
		private PreCompileCommand(string websiteRootDirectory)
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

	    private IPathResolver _pathResolver;
	    public IPathResolver PathResolver
	    {
	        get
	        {
	            if (_pathResolver == null)
	            {
	                _pathResolver = VirtualPathResolver.GetInstance(WebsiteRootDirectory);
	            }

	            return _pathResolver;
	        }
            set { _pathResolver = value; }
	    }

	    /// <summary>
		/// Executes the task.
		/// </summary>
		/// <returns></returns>
		public void Execute()
		{
			LogMessage("Begin consolidating resources...");
			using(new AssemblyResolver(ResolveAssemblyPathToWebsiteBinDir))
            {
                AssmanConfiguration configSection = GetConfigSection(WebsiteRootDirectory);
			    ConsolidateAll(configSection);
			}
			
			LogMessage("End consolidating resources.");
		}

		private void ConsolidateAll(AssmanConfiguration configSection)
		{
			var context = configSection.BuildContext(Mode, usePreCompilationReportIfPresent: false);
			var consolidator = context.GetConsolidator();
			var report = consolidator.CompileAll(WriteCompiledResource);
			report.Version = this.Version ?? DateTime.Now.ToString("yyMMddHHmm");
			configSection.SavePreConsolidationReport(report);
		}

		private void WriteCompiledResource(ICompiledResource compiledResource)
		{
			string consolidatedPath = PathResolver.MapPath(compiledResource.CompiledPath);
			compiledResource.WriteToFile(consolidatedPath);
			LogCompilation(compiledResource);
		}

		private void LogCompilation(ICompiledResource compiledResource)
		{
			LogMessage(String.Format("Compiling '{0}'...", compiledResource.CompiledPath));
			foreach (IResource resource in compiledResource.Resources)
			{
				LogMessage(String.Format("\t...from '{0}'", resource.VirtualPath));
			}
		}

		private void LogMessage(string message)
		{
			_logger.LogMessage(message);
		}

		private AssmanConfiguration GetConfigSection(string webRoot)
		{
			AssmanConfiguration.ConfigLoader = new MappedConfigLoader(webRoot);

			var configSection = AssmanConfiguration.Current;
			configSection.RootFilePath = webRoot;

			return configSection;
		}

		private Assembly ResolveAssemblyPathToWebsiteBinDir(object sender, ResolveEventArgs args)
		{
			var assemblyName = args.Name;
			if (assemblyName.Contains(","))
				assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(","));

			var websiteBinPath = Path.Combine(WebsiteRootDirectory, "bin");

			var assemblyPath = Path.Combine(websiteBinPath, assemblyName + ".dll");
			return Assembly.LoadFrom(assemblyPath);
		}

		private class AssemblyResolver : IDisposable
		{
			private readonly ResolveEventHandler _resolveEventHandler;

			public AssemblyResolver(ResolveEventHandler resolveEventHandler)
			{
				_resolveEventHandler = resolveEventHandler;
				AppDomain.CurrentDomain.AssemblyResolve += _resolveEventHandler;
			}

			public void Dispose()
			{
				AppDomain.CurrentDomain.AssemblyResolve -= _resolveEventHandler;
			}
		}
	}
}