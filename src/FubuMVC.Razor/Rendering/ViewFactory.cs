using System;
using FubuMVC.Razor.RazorEngine;

namespace FubuMVC.Razor.Rendering
{
    public interface IViewFactory
    {
        IFubuRazorView GetView();
        IFubuRazorView GetPartialView();
    }

    public class ViewFactory : IViewFactory
    {
        private readonly IViewEntrySource _viewEntrySource;
        private readonly IViewModifierService _service;

        public ViewFactory(IViewEntrySource viewEntrySource, IViewModifierService service)
        {
            _service = service;
            _viewEntrySource = viewEntrySource;
        }

        public IFubuRazorView GetView()
        {
            return getView(_viewEntrySource.GetViewEntry);
        }

        public IFubuRazorView GetPartialView()
        {
            return getView(_viewEntrySource.GetPartialViewEntry);
        }

        private IFubuRazorView getView(Func<IRazorViewEntry> func)
        {
            var view = (IFubuRazorView)func().CreateInstance();
            view = _service.Modify(view);
            return view;
        }
    }
}