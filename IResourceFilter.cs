using System;

namespace AlmWitt.Web.ResourceManagement
{
	internal interface IResourceFilter
	{
		bool IsMatch(IResource resource);
	}
}
