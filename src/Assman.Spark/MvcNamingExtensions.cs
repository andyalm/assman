using System;
using System.Reflection;
using System.Web.Mvc;

namespace Assman.Spark
{
	public static class MvcNamingExtensions
	{
		public static string ActionName(this MethodInfo methodInfo)
		{
			if (methodInfo.HasAttribute<ActionNameAttribute>())
			{
				return methodInfo.GetAttribute<ActionNameAttribute>().Name;
			}
			else
			{
				return methodInfo.Name;
			}
		}

		public static string ControllerName(this Type controllerType)
		{
			if (controllerType.Name.EndsWith("Controller"))
			{
				return controllerType.Name.Substring(0, controllerType.Name.Length - 10);
			}
			else
			{
				return controllerType.Name;
			}
		}
	}
}