using System.IO;
using System.Web.Mvc;

namespace AlmWitt.Web.ResourceManagement.Mvc
{
	internal static class TagBuilderExtensions
	{
		public static void WriteTo(this TagBuilder tagBuilder, TextWriter writer)
		{
			writer.Write(tagBuilder.ToString());
		}

		public static void WriteStartTag(this TagBuilder tagBuilder, TextWriter writer)
		{
			writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
		}

		public static void WriteEndTag(this TagBuilder tagBuilder, TextWriter writer)
		{
			writer.Write(tagBuilder.ToString(TagRenderMode.EndTag));
		}
	}
}