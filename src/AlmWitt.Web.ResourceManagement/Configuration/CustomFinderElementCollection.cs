using System;
using System.Collections.Generic;
using System.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public class CustomFinderElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new CustomFinderElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			var finderElement = (CustomFinderElement) element;

			return finderElement.Factory ?? String.Empty + finderElement.Type ?? String.Empty;
		}

        public void AddFinderType<TFinder>() where TFinder : IResourceFinder
        {
            BaseAdd(new CustomFinderElement { Type = typeof(TFinder).AssemblyQualifiedName });
        }

        public void AddFinderFactory<TFactory>() where TFactory : IResourceFinderFactory
        {
            BaseAdd(new CustomFinderElement { Factory = typeof(TFactory).AssemblyQualifiedName });
        }

		public IEnumerable<IResourceFinder> GetFinders()
		{
			foreach (CustomFinderElement finderElement in this)
			{
				yield return finderElement.CreateFinder();
			}
		}
	}
}