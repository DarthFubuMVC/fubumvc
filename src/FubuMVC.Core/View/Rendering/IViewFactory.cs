namespace FubuMVC.Core.View.Rendering
{
    public interface IViewFactory
    {
        IRenderableView GetView();
        IRenderableView GetPartialView();
    }
}