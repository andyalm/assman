using System;
using System.IO;
using System.Reflection;

namespace Assman
{
	public static class AssemblyExtensions
	{
		internal static string ToShortAssemblyName(this string assemblyName)
		{
			int commaIndex = assemblyName.IndexOf(",");
			if (commaIndex > 0)
			{
				return assemblyName.Substring(0, commaIndex);
			}
			else
			{
				return assemblyName;
			}
		}

		public static DateTime GetLastWriteTime(this Assembly assembly)
		{
			var assemblyFile = new FileInfo(ToLocationPath(assembly.CodeBase));

			return assemblyFile.LastWriteTime;
		}

		private static string ToLocationPath(string fileUri)
		{
			string uri = fileUri.Substring(8); //strip off file:///
			return uri.Replace("/", @"\");
		}
	}
}