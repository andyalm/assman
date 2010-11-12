using System;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceFilter
	{
		bool IsMatch(IResource resource);
	}
}
