using System;

using AlmWitt.Web.ResourceManagement.Configuration;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace AlmWitt.Web.Test.ResourceManagement
{
	[TestFixture]
	public class TestClientScriptGroupElementCollection
	{
		private int _elementId = 1;
		
		[Test]
		public void SettingCompressPropertyUpdatesCompressDefaultValueOnAllChildren()
		{
			ClientScriptGroupElementCollection collection = new ClientScriptGroupElementCollection();
			collection.Add(CreateGroupElement(false));
			collection.Add(CreateGroupElement(true));
			collection.Add(CreateGroupElement(false));

			collection.Compress = true;

			Assert.That(collection[0].CompressDefaultValue, Is.EqualTo(true));
			Assert.That(collection[1].CompressDefaultValue, Is.EqualTo(true));
			Assert.That(collection[2].CompressDefaultValue, Is.EqualTo(true));
		}

		private ClientScriptGroupElement CreateGroupElement(bool compressDefaultValue)
		{
			var element = new ClientScriptGroupElement();
			element.ConsolidatedUrl = "~/myscript" + _elementId++ + ".js";
			element.CompressDefaultValue = compressDefaultValue;

			return element;
		}
	}
}