using System;

namespace Assman
{
	public interface IResourceMatch
	{
		bool IsMatch();

	    bool MatchesConsolidatedUrl(string consolidatedUrl);

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
			private readonly string _value;

			public FalseResourceMatch(string value)
			{
				_value = value;
			}

			public bool IsMatch()
			{
				return false;
			}

		    public bool MatchesConsolidatedUrl(string consolidatedUrl)
		    {
		        return false;
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

			public bool IsMatch()
			{
				return !_inner.IsMatch();
			}

		    public bool MatchesConsolidatedUrl(string consolidatedUrl)
		    {
		        return !_inner.MatchesConsolidatedUrl(consolidatedUrl);
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