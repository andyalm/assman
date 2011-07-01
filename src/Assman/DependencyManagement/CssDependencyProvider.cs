using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assman.DependencyManagement
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
				   select ToCanonicalPath(match.Groups[1].Value, resource);
		}

		private string ToCanonicalPath(string value, IResource contextResource)
		{
			return value.ToAppRelativePath(contextResource);
		}
	}
}