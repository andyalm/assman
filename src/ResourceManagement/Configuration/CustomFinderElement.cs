using System;
using System.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public class CustomFinderElement : ConfigurationElement
	{
		[ConfigurationProperty(PropertyNames.Factory)]
		public string Factory
		{
			get { return (string) this[PropertyNames.Factory]; }
			set { this[PropertyNames.Factory] = value; }
		}

		[ConfigurationProperty(PropertyNames.Type)]
		public string Type
		{
			get { return (string) this[PropertyNames.Type]; }
			set { this[PropertyNames.Type] = value; }
		}

		public IResourceFinder CreateFinder()
		{
			if (!String.IsNullOrEmpty(this.Type))
			{
				return CreateFinderFromType();
			}
			else if(!String.IsNullOrEmpty(this.Factory))
			{
				return CreateFinderFromFactory();
			}

			throw ConfigException("The custom finder element must either specify the 'type' or 'factory' attribute.");
		}

		private IResourceFinder CreateFinderFromFactory()
		{
			var factoryType = System.Type.GetType(this.Factory);
			AssertTypeImplements<IResourceFinderFactory>(factoryType, "factory");
			var factoryInstance = (IResourceFinderFactory) Activator.CreateInstance(factoryType);

			return factoryInstance.CreateFinder();
		}

		private IResourceFinder CreateFinderFromType()
		{
			var finderType = System.Type.GetType(this.Type);
			AssertTypeImplements<IResourceFinder>(finderType, "type");
			
			return (IResourceFinder) Activator.CreateInstance(finderType);
		}

		private void AssertTypeImplements<TInterface>(Type finderType, string attributeName)
		{
			if(finderType == null)
			{
				throw ConfigException("The custom finder " + attributeName + " '" + this.Type + "' could not be found.");
			}
			if(!typeof(TInterface).IsAssignableFrom(finderType))
			{
				throw ConfigException("The type '" + this.Type + "' must implement the '" +
				                      typeof (IResourceFinder).FullName + "' interface.");
			}
		}

		private Exception ConfigException(string message)
		{
			return new ConfigurationErrorsException(message, this.ElementInformation.Source, this.ElementInformation.LineNumber);
		}

		private static class PropertyNames
		{
			public const string Factory = "factory";
			public const string Type = "type";
		}
	}
}