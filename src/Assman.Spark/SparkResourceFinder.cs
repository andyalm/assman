using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

using System.Linq;

namespace Assman.Spark
{
	public class SparkResourceFinder : IResourceFinder
	{
		private readonly IEnumerable<Assembly> _assemblies;
		private readonly ISparkResourceContentFetcher _contentFetcher;
		private readonly ISparkJavascriptActionFinder _actionFinder;

		public SparkResourceFinder(IEnumerable<Assembly> assemblies, ISparkResourceContentFetcher contentFetcher, ISparkJavascriptActionFinder actionFinder)
		{
			_assemblies = assemblies;
			_contentFetcher = contentFetcher;
			_actionFinder = actionFinder;
		}

		public IEnumerable<IResource> FindResources(ResourceType resourceType)
		{
			if(resourceType != ResourceType.ClientScript)
			{
				return Enumerable.Empty<IResource>();
			}

			var controllerTypes = _assemblies.SelectMany(a => a.GetTypes()).Where(t => t.Is<ControllerBase>());
			return controllerTypes.SelectMany(controllerType => _actionFinder.FindJavascriptActions(controllerType),
			                                                CreateResource);
		}

		public IResource FindResource(string virtualPath)
		{
			//TODO: Test this method (does it even need to be implemented?)  I stubbed it out for fun :-)
			if (!virtualPath.StartsWith("sparkjs://"))
				return null;

			var uri = new Uri(virtualPath);
			var controllerName = uri.Host;
			var controllerType = _assemblies.SelectMany(a => a.GetTypes())
				.Where(t => t.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase)
				            && t.Is<ControllerBase>()).SingleOrDefault();

			if (controllerType == null)
				return null;

			var actionName = uri.PathAndQuery.Substring(1);

			var sparkJsAction = _actionFinder.FindJavascriptActions(controllerType)
				.Where(a => a.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase))
				.SingleOrDefault();
			
			if(sparkJsAction == null)
				return null;

			return new SparkResource(controllerName, sparkJsAction, _contentFetcher);
		}

		private IResource CreateResource(Type controllerType, SparkJavascriptAction action)
		{
			return new SparkResource(controllerType.ControllerName(), action, _contentFetcher);
		}
	}

	internal static class ReflectionExtensions
	{
		public static bool Is<TAbstraction>(this Type type)
		{
			return typeof (TAbstraction).IsAssignableFrom(type);
		}

		public static bool HasAttribute<TAttribute>(this MemberInfo member) where TAttribute : Attribute
		{
			return member.GetAttribute<TAttribute>() != null;
		}

		public static TAttribute GetAttribute<TAttribute>(this MemberInfo member) where TAttribute : Attribute
		{
			var attributes = member.GetCustomAttributes(typeof (TAttribute), true);

			if(attributes.Length == 0)
			{
				return null;
			}
			else
			{
				return (TAttribute)attributes[0];
			}
		}
	}
}