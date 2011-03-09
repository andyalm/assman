using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IDependencyCache
	{
		bool TryGetDependencies(string virtualPath, out IEnumerable<string> dependencies);
		bool TryGetDependencies(IResource resource, out IEnumerable<string> dependencies);
		void StoreDependencies(string virtualPath, IEnumerable<string> dependencies);
		void StoreDependencies(IResource resource, IEnumerable<string> dependencies);
	}
}