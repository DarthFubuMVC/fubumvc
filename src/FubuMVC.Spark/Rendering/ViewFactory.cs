using System;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IViewFactory
    {
        IFubuSparkView GetView();
        IFubuSparkView GetPartialView();
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

        public IFubuSparkView GetView()
        {
            return getView(_viewEntrySource.GetViewEntry);
        }

        public IFubuSparkView GetPartialView()
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