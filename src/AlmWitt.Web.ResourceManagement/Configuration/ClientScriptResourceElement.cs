using System;
using System.Configuration;

using GettyImages.Frameworks.ResourceManagement.ContentFiltering;

namespace GettyImages.Frameworks.ResourceManagement.Configuration
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
		/// Gets or sets whether the scripts will be obvuscated when they are consolidated.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Obvuscate, DefaultValue = false)]
		public bool Obvuscate
		{
			get { return (bool)this[PropertyNames.Obvuscate]; }
			set { this[PropertyNames.Obvuscate] = value; }
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
					ScriptPackerFilter filter = new ScriptPackerFilter();
					filter.Obvuscate = Obvuscate;
					return filter;
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
			public const string Obvuscate = "obvuscate";
		}
	}

	
}