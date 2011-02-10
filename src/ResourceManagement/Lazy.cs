using System;

namespace AlmWitt.Web.ResourceManagement
{
	public class Lazy<T>
	{
		private T _value;
		private bool _valueRetrieved;
		private readonly Func<T> _getValue;

		public Lazy(Func<T> getValue)
		{
			_getValue = getValue;
		}

		public T Value
		{
			get
			{
				if(!_valueRetrieved)
				{
					_value = _getValue();
					_valueRetrieved = true;
				}

				return _value;
			}
		}
	}
}