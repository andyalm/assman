using System;
using System.IO;

using Assman.Registration;

using Spark;
using Spark.Spool;

namespace Assman.Spark.Registration
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