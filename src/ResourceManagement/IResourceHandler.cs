using System;

namespace AlmWitt.Web.ResourceManagement
{
	internal interface IResourceHandler
	{
		DateTime MinLastModified { get; set; }
		void HandleRequest(IRequestContext context);
	}
}