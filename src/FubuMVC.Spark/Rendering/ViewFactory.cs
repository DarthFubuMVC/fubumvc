using System;
using FubuMVC.Core.View.Rendering;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public class ViewFactory : IViewFactory
    {
        private readonly IViewEntrySource _viewEntrySource;
        private readonly IViewModifierService _service;

        public ViewFactory(IViewEntrySource viewEntrySource, IViewModifierService service)
        {
            _service = service;
            _viewEntrySource = viewEntrySource;
        }

        public IRenderableView GetView()
        {
            return getView(_viewEntrySource.GetViewEntry);
        }

        public IRenderableView GetPartialView()
        {
            return getView(_viewEntrySource.GetPartialViewEntry);
        }

        private IRenderableView getView(Func<ISparkViewEntry> func)
        {
            var view = (IRenderableView)func().CreateInstance();
            view = _service.Modify(view);
            return view;
        }
    }
}