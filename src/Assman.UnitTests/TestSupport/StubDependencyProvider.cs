using System;
using System.Collections.Generic;

namespace Assman.TestSupport
{
	public class StubDependencyProvider : IDependencyProvider
	{
		private static readonly string[] _noDependencies = new string[0];
		private readonly Dictionary<string,IEnumerable<string>> _dependencyMap = new Dictionary<string, IEnumerable<string>>(Comparers.VirtualPath);

		public void SetDependencies(IResource resource, params string[] dependencies)
		{
			SetDependencies(resource.VirtualPath, dependencies);
		}
		
		public void SetDependencies(string virtualPath, params string[] dependencies)
		{
			_dependencyMap[virtualPath] = dependencies;
		}
		
		public IEnumerable<string> GetDependencies(IResource resource)
		{
			GetDependenciesWasCalled = true;
			IEnumerable<string> dependencies;
			if (_dependencyMap.TryGetValue(resource.VirtualPath, out dependencies))
				return dependencies;
			else
				return _noDependencies;
		}

		public bool GetDependenciesWasCalled { get; set; }
	}
}