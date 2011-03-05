using System;
using System.Collections.Generic;

using AlmWitt.Web.ResourceManagement.ContentFiltering;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public class ContentFilterMap
	{
		private readonly IDictionary<string,IContentFilterFactory> _map = new Dictionary<string, IContentFilterFactory>();

		public void MapExtension(string fileExtension, IContentFilterFactory filterFactory)
		{
			ValidateFileExtensionArgument(fileExtension);

			_map[fileExtension] = filterFactory;
		}

		public IContentFilterFactory GetFilterFactoryForExtension(string fileExtension)
		{
			ValidateFileExtensionArgument(fileExtension);
			
			if (_map.ContainsKey(fileExtension))
				return _map[fileExtension];
			else
				return NullContentFilterFactory.Instance;
		}

		private void ValidateFileExtensionArgument(string fileExtension)
		{
			if(!fileExtension.StartsWith("."))
			{
				throw new ArgumentException("The fileExtension argument must begine with a dot (e.g. .js, .css)", "fileExtension");
			}
		}
	}
}