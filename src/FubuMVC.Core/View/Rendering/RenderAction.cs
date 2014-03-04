namespace FubuMVC.Core.View.Rendering
{
    public class RenderAction : IRenderAction
    {
        private readonly IViewToken _viewFactory;

        public RenderAction(IViewToken viewFactory)
        {
            _viewFactory = viewFactory;
        }

        public void Render()
        {
            _viewFactory.GetView().Render();
        }

        public void RenderPartial()
        {
            _viewFactory.GetPartialView().Render();
        }
    }
}