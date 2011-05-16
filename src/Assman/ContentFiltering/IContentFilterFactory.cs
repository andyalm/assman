using System;

namespace Assman.ContentFiltering
{
	public interface IContentFilterFactory
	{
		IContentFilter CreateFilter(ResourceContentSettings settings);
	}
}