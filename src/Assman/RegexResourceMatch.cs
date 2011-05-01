using System.Text.RegularExpressions;

namespace Assman
{
	public class RegexResourceMatch : IResourceMatch
	{
		private readonly Match _match;
		private readonly ResourceMode? _mode;

		public RegexResourceMatch(Match match, ResourceMode? mode)
		{
			_match = match;
			_mode = mode;
		}

		public bool IsMatch()
		{
			return _match.Success;
		}

		public bool IsMatch(ResourceMode mode)
		{
			return IsMatch() && (_mode == null || _mode == mode);
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