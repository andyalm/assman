using System.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	/// <summary>
	/// Represents a collection of <see cref="ClientScriptGroupElement"/>.
	/// </summary>
	public class ClientScriptGroupElementCollection : ResourceGroupElementCollection<ClientScriptGroupElement>
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
		/// 
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			ClientScriptGroupElement element = new ClientScriptGroupElement();
			//set the compress and obvuscate properties so that they default to the values
			//set by their parent (i.e. this collection).
			element.Compress = Compress;

			return element;
		}

		private static class PropertyNames
		{
			public const string Obvuscate = "obvuscate";
			public const string Compress = "compress";
		}
	}
}