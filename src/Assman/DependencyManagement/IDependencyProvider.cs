using System;
using System.Collections.Generic;

namespace Assman.DependencyManagement
{
	public interface IDependencyProvider
	{
		IEnumerable<string> GetDependencies(IResource resource);
	}
}