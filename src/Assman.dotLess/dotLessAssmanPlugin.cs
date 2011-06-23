using System;

using Assman.Configuration;
using Assman.ContentFiltering;

namespace Assman.dotLess
{
	public class dotLessAssmanPlugin : IAssmanPlugin
	{
		private const string LessFileExtension = ".less";
		
		public void Initialize(AssmanContext context)
		{
			ResourceType.Stylesheet.AddFileExtension(LessFileExtension);
			var lessPipeline = new ContentFilterPipeline();
			lessPipeline.Add(new LessContentFilter());
			context.MapExtensionToContentPipeline(LessFileExtension, lessPipeline);
			context.MapExtensionToDependencyProvider(LessFileExtension, CssDependencyProvider.GetInstance());
		}
	}
}