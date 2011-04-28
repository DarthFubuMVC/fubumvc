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
            var view = _viewFactory.GetView();
            view.RenderView(_nestedOutput.Writer);
        }
    }

    public class DefaultRenderAction : IRenderAction
    {
        private readonly IViewFactory _viewFactory;
        private readonly NestedOutput _nestedOutput;
        private readonly ViewOutput _viewOutput;

        public DefaultRenderAction(IViewFactory viewFactory, NestedOutput nestedOutput, ViewOutput viewOutput)
        {
            _viewFactory = viewFactory;
            _viewOutput = viewOutput;
            _nestedOutput = nestedOutput;
        }

        public void Render()
        {
            var view = (IFubuSparkView)_viewFactory.GetView();
            _nestedOutput.SetWriter(() => view.Output);
            view.RenderView(_viewOutput);
        }
    }
}