using System.Web;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor.Rendering
{
    public interface IPartialRenderer
    {
        HtmlString Render(IFubuRazorView view, string name);
        HtmlString Render(IFubuRazorView view, string name, object model);
    }

    public class PartialRenderer : IPartialRenderer
    {
        private readonly ISharedTemplateLocator<IRazorTemplate> _sharedTemplateLocator;
        private readonly ITemplateFactory _templateFactory;

        public PartialRenderer(ISharedTemplateLocator<IRazorTemplate> sharedTemplateLocator, ITemplateFactory templateFactory)
        {
            _sharedTemplateLocator = sharedTemplateLocator;
            _templateFactory = templateFactory;
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
            var template = _sharedTemplateLocator.LocatePartial(name, view.OriginTemplate);
            var partialView = _templateFactory.GetView(template.Descriptor.As<ViewDescriptor<IRazorTemplate>>());
            return partialView;
        }
    }
}