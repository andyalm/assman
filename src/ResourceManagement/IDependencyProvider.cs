using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IDependencyProvider
	{
		IEnumerable<string> GetDependencies(IResource resource);
	}
}