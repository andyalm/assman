using System;

namespace Assman
{
	public class PathResourceMatch : IResourceMatch
	{
		private readonly string _pathToMatch;
		private readonly string _actualValue;

	    public PathResourceMatch(string pathToMatch, string actualValue)
		{
			_pathToMatch = pathToMatch;
			_actualValue = actualValue;
		}

		public bool IsMatch()
		{
			return _pathToMatch.EqualsVirtualPath(_actualValue);
		}

	    public string GetSubValue(string name)
		{
			return null;
		}

		public string Value
		{
			get
			{
				if (IsMatch())
					return _actualValue;
				else
					return null;
			}
		}
	}
}