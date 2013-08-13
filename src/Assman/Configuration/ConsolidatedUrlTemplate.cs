using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assman.Configuration
{
	internal class ConsolidatedUrlTemplate
	{
		private readonly string _consolidatedUrlString;
		private readonly List<string> _placeholders;
		private static readonly Regex _templateItemRegex = new Regex(@"\{(?<name>\w+)\}", RegexOptions.Compiled);
		private Regex _matchRegex;

		public static ConsolidatedUrlTemplate GetInstance(string consolidatedUrlString)
		{
			return new ConsolidatedUrlTemplate(consolidatedUrlString);
		}

		private ConsolidatedUrlTemplate(string consolidatedUrlString)
		{
			_consolidatedUrlString = consolidatedUrlString;
			_placeholders = (from match in _templateItemRegex.Matches(consolidatedUrlString).Cast<Match>()
                            select match.Groups["name"].Value).ToList();
		}

		public bool HasParameters
		{
			get { return _placeholders.Count > 0; }
		}

		public string Format(IResourceMatch match)
		{
			if (!HasParameters)
				return _consolidatedUrlString;

			var formattedString = _consolidatedUrlString;
			foreach (var placeholder in _placeholders)
			{
				var replacementValue = match.GetSubValue(placeholder);
				formattedString = formattedString.Replace("{" + placeholder + "}", replacementValue);
			}

			return formattedString;
		}

		public bool Matches(string consolidatedUrl)
		{
			if (!HasParameters)
				return _consolidatedUrlString.EqualsVirtualPath(consolidatedUrl);

			return MatchRegex.IsMatch(consolidatedUrl);
		}

	    public bool Matches(IResourceMatch resourceMatch)
	    {
	        return _placeholders.All(resourceMatch.HasSubValue);
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