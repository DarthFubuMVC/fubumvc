using System.IO;
using FubuMVC.Core.Runtime;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IPartialRenderAction
    {
        void Execute();
    }

    public class PartialRenderAction : IPartialRenderAction
    {
        private readonly ISparkViewProvider _provider;
        private readonly PartialOutput _partialOutput;

        public PartialRenderAction(ISparkViewProvider provider, PartialOutput partialOutput )
        {
            _provider = provider;
            _partialOutput = partialOutput;
        }

        public void Execute()
        {
            var view = _provider.GetView();
            view.RenderView(_partialOutput.Writer);
        }
    }

    public interface IRenderAction
    {
        void Execute();
    }

    public class RenderAction : IRenderAction
    {
        private readonly ISparkViewProvider _provider;
        private readonly IOutputWriter _outputWriter;
        private readonly PartialOutput _partialOutput;

        public RenderAction(ISparkViewProvider provider, IOutputWriter outputWriter, PartialOutput partialOutput)
        {
            _provider = provider;
            _partialOutput = partialOutput;
            _outputWriter = outputWriter;
        }

        public void Execute()
        {
            var writer = new StringWriter();
            var view = (SparkViewBase)_provider.GetView();

            _partialOutput.SetWriter(() => view.Output);
            view.Output = writer;
            view.RenderView(writer);

            _outputWriter.WriteHtml(writer);
        }
    }
}