using System.IO;
using FubuMVC.Core.Runtime;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IRenderAction
    {
        void Render();
    }

    public class NestedRenderAction : IRenderAction
    {
        private readonly IViewFactory _viewFactory;
        private readonly NestedOutput _nestedOutput;

        public NestedRenderAction(IViewFactory viewFactory, NestedOutput nestedOutput)
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

    public class DefaultRenderAction : IRenderAction
    {
        private readonly IViewFactory _viewFactory;
        private readonly NestedOutput _nestedOutput;
        private readonly IOutputWriter _outputWriter;

        public DefaultRenderAction(IViewFactory viewFactory, NestedOutput nestedOutput, IOutputWriter outputWriter)
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
            
            // let's see if we can get away from this. Generalize to a sink.
            _outputWriter.WriteHtml(writer);
        }
    }
}