using System;
using System.Text.RegularExpressions;

namespace Assman
{
	public class RegexResourceMatch : IResourceMatch
	{
		private readonly Match _match;

	    public RegexResourceMatch(Match match)
		{
			_match = match;
		}

		public bool IsMatch()
		{
			return _match.Success;
		}

	    public bool HasSubValue(string name)
	    {
	        return _match.Groups[name].Success;
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