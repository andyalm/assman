using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	/// <summary>
	/// Represents a pattern to match against resources.
	/// </summary>
	public class ResourceMatchElement : ConfigurationElement
	{
		private Regex _rx;
		
		/// <summary>
		/// Gets or sets the regex pattern used to match.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Pattern, IsRequired = true, IsKey = true)]
		public string Pattern
		{
			get { return this[PropertyNames.Pattern] as string;}
			set { this[PropertyNames.Pattern] = value; }
		}
		
		/// <summary>
		/// Gets whether the given resource path is a match against the pattern.
		/// </summary>
		/// <param name="resourcePath"></param>
		/// <returns></returns>
		public bool IsMatch(string resourcePath)
		{
			return Rx.IsMatch(resourcePath);
		}

		public IResourceMatch GetMatch(string resourcePath)
		{
			return new RegexResourceMatch(Rx.Match(resourcePath));
		}

		private Regex Rx
		{
			get
			{
				if(_rx == null)
					_rx = new Regex(Pattern, RegexOptions.IgnoreCase);
				return _rx;
			}
		}

		private static class PropertyNames
		{
			public const string Pattern = "pattern";
		}
	}
}
