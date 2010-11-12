namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceMatch
	{
		bool IsMatch { get; }

		string GetSubValue(string name);

		string Value { get; }
	}

	public static class ResourceMatches
	{
		public static IResourceMatch False(string value)
		{
			return new FalseResourceMatch(value);
		}

		public static IResourceMatch Inverse(this IResourceMatch match)
		{
			return new InverseResourceMatch(match);
		}

		private class FalseResourceMatch : IResourceMatch
		{
			private string _value;

			public FalseResourceMatch(string value)
			{
				_value = value;
			}

			public bool IsMatch
			{
				get { return false; }
			}

			public string GetSubValue(string name)
			{
				return null;
			}

			public string Value
			{
				get { return _value; }
			}
		}

		private class InverseResourceMatch : IResourceMatch
		{
			private readonly IResourceMatch _inner;

			public InverseResourceMatch(IResourceMatch inner)
			{
				_inner = inner;
			}

			public bool IsMatch
			{
				get { return !_inner.IsMatch; }
			}

			public string GetSubValue(string name)
			{
				return _inner.GetSubValue(name);
			}

			public string Value
			{
				get { return _inner.Value; }
			}
		}
	}
}