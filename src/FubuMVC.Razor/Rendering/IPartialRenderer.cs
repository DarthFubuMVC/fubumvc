using System.Web;
using FubuCore.Util;
using FubuMVC.Core.View;

namespace FubuMVC.Razor.Rendering
{
    public interface IPartialRenderer
    {
        HtmlString Render(IFubuRazorView view, string name);
        HtmlString Render(IFubuRazorView view, string name, object model);
    }

    public class PartialRenderer : IPartialRenderer
    {
        private readonly ViewEngineSettings _views;

        public PartialRenderer(ViewEngineSettings views)
        {
            _views = views;
        }

        public HtmlString Render(IFubuRazorView view, string name)
        {
            var partialView = getPartialView(view, name);
            partialView.Execute();
            return new HtmlString(partialView.Result.ToString());
        }

        public HtmlString Render(IFubuRazorView view, string name, object model)
        {
            var partialView = getPartialView(view, name);
            dynamic fubuPage = partialView;
            dynamic dynamicModel = model;
            fubuPage.Model = dynamicModel;
            partialView.Execute();
            return new HtmlString(partialView.Result.ToString());
        }

        private IFubuRazorView getPartialView(IFubuRazorView view, string name)
        {
            var template = _views.FindPartial(view.OriginTemplate, name);
            return template.GetPartialView() as IFubuRazorView;
        }
    }
}