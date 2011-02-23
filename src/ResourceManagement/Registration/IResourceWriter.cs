using System.IO;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public interface IResourceWriter
	{
		void WriteIncludeTag(TextWriter writer, string url);
		void WriteBeginBlock(TextWriter writer);
		void WriteEndBlock(TextWriter writer);
	}
}