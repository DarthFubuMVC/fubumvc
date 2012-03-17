using System;
using FubuMVC.Core.View.Rendering;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public class ViewFactory : IViewFactory
    {
        private readonly IViewEntrySource _viewEntrySource;
        private readonly IViewModifierService<IFubuSparkView> _service;

        public ViewFactory(IViewEntrySource viewEntrySource, IViewModifierService<IFubuSparkView> service)
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

        private IFubuSparkView getView(Func<ISparkViewEntry> func)
        {
            var view = (IFubuSparkView)func().CreateInstance();
            view = _service.Modify(view);
            return view;
        }
    }
}