using System;
using System.Collections.Generic;
using System.IO;

using Assman.Configuration;

using Spark;
using Spark.FileSystem;
using Spark.Web.Mvc;

namespace Assman.Spark
{
	public class SparkResourceContentFetcher : ISparkResourceContentFetcher
	{
		private ISparkSettings _settings;
		private ISparkViewEngine _sparkViewEngine;
		private IDescriptorBuilder _descriptorBuilder;

		public SparkResourceContentFetcher(ISparkSettings settings)
		{
			_settings = settings;
			_sparkViewEngine = new SparkViewEngine(_settings);
			_sparkViewEngine.ViewFolder =
				new FileSystemViewFolder(Path.Combine(ResourceManagementConfiguration.Current.RootFilePath, "Views"));
			_descriptorBuilder = new DefaultDescriptorBuilder(_sparkViewEngine);
		}

		public string GetContent(string controllerName, string viewName, string masterName)
		{
			var viewDescriptor = GetViewDescriptor(controllerName, viewName, masterName);

			var viewEntry = _sparkViewEngine.CreateEntry(viewDescriptor);

			return viewEntry.SourceCode;
		}

		private SparkViewDescriptor GetViewDescriptor(string controllerName, string viewName, string masterName)
		{
			var searchLocations = new List<string>();
			
			var buildDescriptorParams = new BuildDescriptorParams(String.Empty, controllerName, viewName, masterName, false,
			                                                      new Dictionary<string, object>());

			var viewDescriptor = _descriptorBuilder.BuildDescriptor(buildDescriptorParams, searchLocations);
			viewDescriptor.Language = LanguageType.Javascript;
			
			return viewDescriptor;
		}
	}
}