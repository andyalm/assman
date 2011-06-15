using System;

namespace Assman.Mvc.Registration
{
	public class DummyStringResult
	{
		private static readonly DummyStringResult _instance = new DummyStringResult();

		public static DummyStringResult Instance
		{
			get { return _instance; }
		}

		private DummyStringResult() {}
		
		public override string ToString()
		{
			return string.Empty;
		}
	}
}