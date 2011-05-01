using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assman
{
	public class CssDependencyProvider : IDependencyProvider
	{
		public static CssDependencyProvider GetInstance()
		{
			return new CssDependencyProvider();
		}
		
		private static readonly Regex _dependencyRegex = new Regex(@"dependency\s*\:\s*url\(([^)]+)\)", RegexOptions.Compiled);
		
		internal CssDependencyProvider() {}
		
		public IEnumerable<string> GetDependencies(IResource resource)
		{
			return from match in _dependencyRegex.Matches(resource.GetContent()).Cast<Match>()
			       where match.Success
			       select ToCanonicalPath(match.Groups[1].Value);
		}

		private string ToCanonicalPath(string value)
		{
			if (value.StartsWith("/"))
				return "~" + value;
			else
				return value;
		}
	}
}