using AlmWitt.Web.ResourceManagement;

namespace AlmWitt.Web.Test.ResourceManagement.TestObjects
{
	public class StubResourceType : ResourceType
	{
		private readonly string _contentType;
		private readonly string _defaultFileExtension;

		public StubResourceType(string contentType, string defaultFileExtension)
		{
			_contentType = contentType;
			_defaultFileExtension = defaultFileExtension;
		}

		public override string ContentType
		{
			get { return _contentType; }
		}

		public override string DefaultFileExtension
		{
			get { return _defaultFileExtension; }
		}
	}
}