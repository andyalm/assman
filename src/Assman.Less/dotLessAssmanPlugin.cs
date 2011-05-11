using Assman.Configuration;

namespace Assman.Less
{
	public class dotLessAssmanPlugin : IAssmanPlugin
	{
		private const string LessFileExtension = ".less";
		
		public void Initialize(AssmanContext context)
		{
			ResourceType.Stylesheet.AddFileExtension(LessFileExtension);
			context.MapExtensionToFilter(LessFileExtension, new LessContentFilterFactory());
			context.MapExtensionToDependencyProvider(LessFileExtension, CssDependencyProvider.GetInstance());
		}
	}
}