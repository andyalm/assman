using System;

using Assman.Configuration;

using NUnit.Framework;

namespace Assman
{
	[TestFixture]
	public class TestClientScriptGroupElementCollection
	{
		private int _elementId = 1;
		
		[Test]
		public void SettingMinifyPropertyUpdatesMinifyDefaultValueOnAllChildren()
		{
			ScriptGroupElementCollection collection = new ScriptGroupElementCollection();
			collection.Add(CreateGroupElement(false));
			collection.Add(CreateGroupElement(true));
			collection.Add(CreateGroupElement(false));

			collection.Minify = true;

			Assert.That(collection[0].MinifyDefaultValue, Is.EqualTo(true));
			Assert.That(collection[1].MinifyDefaultValue, Is.EqualTo(true));
			Assert.That(collection[2].MinifyDefaultValue, Is.EqualTo(true));
		}

		private ScriptGroupElement CreateGroupElement(bool minifyDefaultValue)
		{
			var element = new ScriptGroupElement();
			element.ConsolidatedUrl = "~/myscript" + _elementId++ + ".js";
			element.MinifyDefaultValue = minifyDefaultValue;

			return element;
		}
	}
}