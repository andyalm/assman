using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

using System.Linq;

namespace AlmWitt.Web.ResourceManagement.Spark
{
	public class SparkResourceFinder : IResourceFinder
	{
		private static readonly ResourceCollection _emptyResourceCollection = new ResourceCollection();
		
		private readonly IEnumerable<Assembly> _assemblies;
		private readonly ISparkResourceContentFetcher _contentFetcher;
		private readonly ISparkJavascriptActionFinder _actionFinder;

		public SparkResourceFinder(IEnumerable<Assembly> assemblies, ISparkResourceContentFetcher contentFetcher, ISparkJavascriptActionFinder actionFinder)
		{
			_assemblies = assemblies;
			_contentFetcher = contentFetcher;
			_actionFinder = actionFinder;
		}

		public ResourceCollection FindResources(string extension)
		{
			if(!extension.EndsWith(".js"))
			{
				return _emptyResourceCollection;
			}

			var controllerTypes = _assemblies.SelectMany(a => a.GetTypes()).Where(t => t.Is<ControllerBase>());
			var sparkResources = controllerTypes.SelectMany(controllerType => _actionFinder.FindJavascriptActions(controllerType),
			                                                (controllerType, action) => CreateResource(controllerType, action));
			
			return new ResourceCollection(sparkResources);
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