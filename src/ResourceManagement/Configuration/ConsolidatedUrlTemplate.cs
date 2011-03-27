using System;
using System.Text.RegularExpressions;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	internal class ConsolidatedUrlTemplate
	{
		private readonly string _consolidatedUrlString;
		private readonly MatchCollection _matches;
		private static readonly Regex _templateItemRegex = new Regex(@"\{(?<name>\w+)\}", RegexOptions.Compiled);
		private Regex _matchRegex;

		public static ConsolidatedUrlTemplate GetInstance(string consolidatedUrlString)
		{
			return new ConsolidatedUrlTemplate(consolidatedUrlString);
		}

		private ConsolidatedUrlTemplate(string consolidatedUrlString)
		{
			_consolidatedUrlString = consolidatedUrlString;
			_matches = _templateItemRegex.Matches(consolidatedUrlString);
		}

		public bool HasParameters
		{
			get { return _matches.Count > 0; }
		}

		public string Format(IResourceMatch match)
		{
			if (!HasParameters)
				return _consolidatedUrlString;

			var formattedString = _consolidatedUrlString;
			foreach (Match rxMatch in _matches)
			{
				var name = rxMatch.Groups["name"].Value;
				var replacementValue = match.GetSubValue(name);
				formattedString = formattedString.Replace("{" + name + "}", replacementValue);
			}

			return formattedString;
		}

		public bool Matches(string consolidatedUrl)
		{
			if (!HasParameters)
				return _consolidatedUrlString.Equals(consolidatedUrl, StringComparison.OrdinalIgnoreCase);

			return MatchRegex.IsMatch(consolidatedUrl);
		}

		private Regex MatchRegex
		{
			get
			{
				if(_matchRegex == null)
				{
					var rxString = _templateItemRegex.Replace(_consolidatedUrlString, @"\w+");
					_matchRegex = new Regex(rxString);
				}

				return _matchRegex;
			}
		}
	}
}