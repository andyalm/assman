
using System;

namespace Assman.ContentFiltering
{
	internal class NullContentFilter : IContentFilter
	{
		private static readonly NullContentFilter _instance = new NullContentFilter();

		public static NullContentFilter Instance
		{
			get { return _instance; }
		}

		private NullContentFilter() {}

		public string FilterContent(string content, ContentFilterContext context)
		{
			return content;
		}
	}
}