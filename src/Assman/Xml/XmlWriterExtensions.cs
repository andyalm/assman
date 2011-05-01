using System;
using System.Xml;

namespace Assman.Xml
{
	public static class XmlWriterExtensions
	{
		public static IDisposable Document(this XmlWriter writer)
		{
		 	writer.WriteStartDocument();
		 	return new Disposable(writer.WriteEndDocument);
		}

		public static IDisposable Element(this XmlWriter writer, string elementName, params Func<string,string>[] attributes)
		{
			writer.WriteStartElement(elementName);
			foreach (var attribute in attributes)
			{
				var name = attribute.Method.GetParameters()[0].Name;
				var value = attribute(name);
				writer.WriteAttributeString(name, value);
			}
			return new Disposable(writer.WriteEndElement);
		}

		private class Disposable : IDisposable
		{
			private readonly Action _onDispose;

			public Disposable(Action onDispose)
			{
				_onDispose = onDispose;
			}

			public void Dispose()
			{
				_onDispose();
			}
		}
	}
}