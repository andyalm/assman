using System;
using System.IO;

namespace AlmWitt.Web.ResourceManagement
{
	public static class TextWriterExtensions
	{
		public static string RenderToString(this Action<TextWriter> block)
		{
			using (var writer = new StringWriter())
			{
				block(writer);
				return writer.ToString();
			}
		}
	}
}