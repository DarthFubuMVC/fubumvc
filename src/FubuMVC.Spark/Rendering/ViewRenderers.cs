using System.IO;
using FubuMVC.Core.Runtime;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    // NOTE: CONFUSING NAME, THERE'S A SPARKVIEWRENDERER (THE BEHAVIOR)
    // TODO: RENAME
    public interface IViewRenderer
    {
        void Render();
    }
    public class NestedViewRenderer : IViewRenderer
    {
        private readonly IViewFactory _viewFactory;
        private readonly NestedOutput _nestedOutput;

        public NestedViewRenderer(IViewFactory viewFactory, NestedOutput nestedOutput)
        {
            _viewFactory = viewFactory;
            _nestedOutput = nestedOutput;
        }

        public void Render()
        {
            var partial = _viewFactory.GetView();
            partial.RenderView(_nestedOutput.Writer);
        }
    }
    public class DefaultViewRenderer : IViewRenderer
    {
        private readonly IViewFactory _viewFactory;
        private readonly NestedOutput _nestedOutput;
        private readonly IOutputWriter _outputWriter;

        public DefaultViewRenderer(IViewFactory viewFactory, NestedOutput nestedOutput, IOutputWriter outputWriter)
        {
            _viewFactory = viewFactory;
            _outputWriter = outputWriter;
            _nestedOutput = nestedOutput;
        }

        public void Render()
        {
            var view = (SparkViewBase)_viewFactory.GetView();
            var writer = new StringWriter();
            view.Output = writer;
            _nestedOutput.SetWriter(() => view.Output);
            view.RenderView(writer);
            _outputWriter.WriteHtml(writer);
        }
    }
}