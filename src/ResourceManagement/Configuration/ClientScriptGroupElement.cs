using System;
using System.Configuration;
using AlmWitt.Web.ResourceManagement.ContentFiltering;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	/// <summary>
	/// A <see cref="ResourceGroupElement"/> used to configure client scripts.
	/// </summary>
	public class ClientScriptGroupElement : ResourceGroupElement
	{
		/// <summary>
		/// Gets or sets whether the scripts will be compressed when they are consolidated.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Compress, DefaultValue = false)]
		public bool Compress
		{
			get { return (bool)this[PropertyNames.Compress]; }
			set { this[PropertyNames.Compress] = value; }
		}

		/// <summary>
		/// Gets the content filter
		/// </summary>
		protected override IContentFilter ContentFilter
		{
			get
			{
				if (Compress)
				{
					return new JSMinFilter();
				}
				else
				{
					return base.ContentFilter;
				}
			}
		}

		private static class PropertyNames
		{
			public const string Compress = "compress";
		}
	}


}