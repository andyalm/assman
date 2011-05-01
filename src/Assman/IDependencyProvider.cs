using System.Collections.Generic;

namespace Assman
{
	public interface IDependencyProvider
	{
		IEnumerable<string> GetDependencies(IResource resource);
	}
}