using System;

namespace Assman
{
	internal interface IResourceHandler
	{
		DateTime MinLastModified { get; set; }
		void HandleRequest(IRequestContext context);
	}
}