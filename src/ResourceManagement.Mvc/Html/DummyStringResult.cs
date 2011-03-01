using System.Web;

namespace AlmWitt.Web.ResourceManagement.Mvc
{
	public class DummyStringResult : IHtmlString
	{
		private static readonly DummyStringResult _instance = new DummyStringResult();

		public static DummyStringResult Instance
		{
			get { return _instance; }
		}

		private DummyStringResult() {}
		
		public string ToHtmlString()
		{
			return string.Empty;
		}

		public override string ToString()
		{
			return string.Empty;
		}
	}
}