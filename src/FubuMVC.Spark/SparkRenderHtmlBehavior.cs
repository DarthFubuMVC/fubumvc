using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Spark.Rendering;
using HtmlTags;

namespace FubuMVC.Spark
{
	public class SparkHtmlDocumentOutput : OutputNode<SparkRenderHtmlBehavior<HtmlDocument>>
	{
		public override string Description { get { return "Render Html Document with Spark"; } }
	}

	public class SparkHtmlTagOutput : OutputNode<SparkRenderHtmlBehavior<HtmlTag>>
	{
		public override string Description { get { return "Render Html Tag with Spark"; } }
	}

	public class SparkRenderHtmlBehavior<T> : IActionBehavior where T : class 
	{
		private readonly IOutputWriter _outputWriter;
		private readonly  NestedOutput _nestedOutput;
		private readonly IFubuRequest _request;

		public SparkRenderHtmlBehavior(IOutputWriter outputWriter, NestedOutput nestedOutput, IFubuRequest request)
		{
			_outputWriter = outputWriter;
			_nestedOutput = nestedOutput;
			_request = request;
		}

		public void Invoke()
		{
			var outputType = _request.Get<T>();
			_outputWriter.WriteHtml(outputType);
		}

		public void InvokePartial()
		{
			var tag = _request.Get<T>();
			_nestedOutput.Writer.Write(tag);
		}
	}
}