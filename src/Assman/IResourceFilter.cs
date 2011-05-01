using System;

namespace Assman
{
	public interface IResourceFilter
	{
		bool IsMatch(IResource resource);
	}
}
