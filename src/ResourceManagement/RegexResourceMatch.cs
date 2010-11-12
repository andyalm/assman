using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AlmWitt.Web.ResourceManagement
{
	public class RegexResourceMatch : IResourceMatch
	{
		private readonly Match _match;

		public RegexResourceMatch(Match match)
		{
			_match = match;
		}

		public bool IsMatch
		{
			get { return _match.Success; }
		}

		public string GetSubValue(string name)
		{
			var group = _match.Groups[name];
			if (group.Success)
				return group.Value;
			else
				return null;
		}

		public string Value
		{
			get { return _match.Value; }
		}
	}
}