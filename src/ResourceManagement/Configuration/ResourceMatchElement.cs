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
		[ConfigurationProperty(PropertyNames.Pattern, IsRequired = false, IsKey = false)]
		public string Pattern
		{
			get { return this[PropertyNames.Pattern] as string;}
			set { this[PropertyNames.Pattern] = value; }
		}

		/// <summary>
		/// Gets or sets the regex pattern used to match.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Path, IsRequired = false, IsKey = false)]
		public string Path
		{
			get { return this[PropertyNames.Path] as string; }
			set { this[PropertyNames.Path] = value; }
		}

		[ConfigurationProperty(PropertyNames.Mode, DefaultValue = null)]
		public ResourceMode? Mode
		{
			get { return (ResourceMode?) this[PropertyNames.Mode]; }
			set { this[PropertyNames.Mode] = value; }
		}

		public string Key
		{
			get
			{
				if (IsRegexMode)
					return Pattern;
				else
					return Path;
			}
		}
		
		/// <summary>
		/// Gets whether the given resource path is a match against the pattern.
		/// </summary>
		/// <param name="resourcePath"></param>
		/// <returns></returns>
		public bool IsMatch(string resourcePath)
		{
			return GetMatch(resourcePath).IsMatch();
		}

		public IResourceMatch GetMatch(string resourcePath)
		{
			if (IsRegexMode)
				return new RegexResourceMatch(Rx.Match(resourcePath), Mode);
			else
				return new PathResourceMatch(Path, resourcePath, Mode);
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

		private bool IsRegexMode
		{
			get { return String.IsNullOrEmpty(Path); }
		}
	}
}
