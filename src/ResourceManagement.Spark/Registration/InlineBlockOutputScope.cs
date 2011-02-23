using System;
using System.IO;

using AlmWitt.Web.ResourceManagement.Registration;

using Spark;
using Spark.Spool;

namespace AlmWitt.Web.ResourceManagement.Spark.Registration
{
	public class InlineBlockOutputScope : IDisposable
	{
		private readonly SparkViewBase _sparkView;
		private readonly IResourceRegistry _resourceRegistry;
		private readonly TextWriter _previousWriter;
		private readonly SpoolWriter _blockWriter;
		
		public InlineBlockOutputScope(SparkViewBase sparkView, IResourceRegistry resourceRegistry)
		{
			_sparkView = sparkView;
			_resourceRegistry = resourceRegistry;
			_previousWriter = sparkView.Output;
			_blockWriter = new SpoolWriter();
			_sparkView.Output = _blockWriter;
		}

		public void Dispose()
		{
			_resourceRegistry.RegisterInlineBlock(writer => _blockWriter.WriteTo(writer));
			_sparkView.Output = _previousWriter;
		}
	}
}