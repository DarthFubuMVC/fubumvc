using FubuCore.Descriptions;

namespace FubuMVC.Core.View.Rendering
{
    public interface IViewFactory : DescribesItself
    {
        IRenderableView GetView();
        IRenderableView GetPartialView();
    }
}