using System;

namespace AlmWitt.Web.ResourceManagement
{
	public class PathResourceMatch : IResourceMatch
	{
		private readonly string _pathToMatch;
		private readonly string _actualValue;
		private readonly ResourceMode? _mode;

		public PathResourceMatch(string pathToMatch, string actualValue, ResourceMode? mode)
		{
			_pathToMatch = pathToMatch;
			_actualValue = actualValue;
			_mode = mode;
		}

		public bool IsMatch()
		{
			return _pathToMatch.Equals(_actualValue, StringComparison.OrdinalIgnoreCase);
		}

		public bool IsMatch(ResourceMode mode)
		{
			return IsMatch() && (_mode == null || _mode == mode);
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