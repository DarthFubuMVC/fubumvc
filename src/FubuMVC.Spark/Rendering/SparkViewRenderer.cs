namespace FubuMVC.Spark.Rendering
{
    public interface ISparkViewRenderer
    {
        void Render();
    }

    public class SparkViewRenderer : ISparkViewRenderer
    {
        private readonly IPartialRenderAction _partialAction;
        private readonly IRenderAction _renderAction;
        private readonly PartialOutput _partialOutput;

        public SparkViewRenderer(IPartialRenderAction partialAction, IRenderAction renderAction, PartialOutput partialOutput)
        {
            _partialAction = partialAction;
            _renderAction = renderAction;
            _partialOutput = partialOutput;
        }

        public void Render()
        {
            if (_partialOutput.IsActive())
            {
                _partialAction.Execute();
            }
            else
            {
                _renderAction.Execute();
            }
        }
    }
}