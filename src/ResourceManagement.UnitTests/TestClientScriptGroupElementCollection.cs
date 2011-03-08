using System;

using AlmWitt.Web.ResourceManagement.Configuration;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestClientScriptGroupElementCollection
	{
		private int _elementId = 1;
		
		[Test]
		public void SettingMinifyPropertyUpdatesMinifyDefaultValueOnAllChildren()
		{
			ClientScriptGroupElementCollection collection = new ClientScriptGroupElementCollection();
			collection.Add(CreateGroupElement(false));
			collection.Add(CreateGroupElement(true));
			collection.Add(CreateGroupElement(false));

			collection.Minify = true;

			Assert.That(collection[0].MinifyDefaultValue, Is.EqualTo(true));
			Assert.That(collection[1].MinifyDefaultValue, Is.EqualTo(true));
			Assert.That(collection[2].MinifyDefaultValue, Is.EqualTo(true));
		}

		private ClientScriptGroupElement CreateGroupElement(bool minifyDefaultValue)
		{
			var element = new ClientScriptGroupElement();
			element.ConsolidatedUrl = "~/myscript" + _elementId++ + ".js";
			element.MinifyDefaultValue = minifyDefaultValue;

			return element;
		}
	}
}