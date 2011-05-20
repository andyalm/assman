using System;
using System.Collections.Generic;

using Assman.ContentFiltering;

namespace Assman.Configuration
{
	public class ContentFilterPipelineMap
	{
		private readonly IDictionary<string,ContentFilterPipeline> _map = new Dictionary<string, ContentFilterPipeline>();

		public void MapExtension(string fileExtension, ContentFilterPipeline pipeline)
		{
			ValidateFileExtensionArgument(fileExtension);

			_map[fileExtension] = pipeline;
		}

		public ContentFilterPipeline GetPipelineForExtension(string fileExtension)
		{
			ValidateFileExtensionArgument(fileExtension);

		    ContentFilterPipeline pipeline;
		    if (!_map.TryGetValue(fileExtension, out pipeline))
		    {
		        pipeline = new ContentFilterPipeline();
		        _map[fileExtension] = pipeline;
		    }
		    return pipeline;
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