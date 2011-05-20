using System;
using System.Collections.Generic;
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
        private readonly IEnumerable<IViewModifier> _modifications;

        public ViewFactory(IViewEntrySource viewEntrySource, IEnumerable<IViewModifier> modifications)
        {
            _modifications = modifications;
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
            view = applyModifications(view);
            return view;
        }

        private IFubuSparkView applyModifications(IFubuSparkView view)
        {
            foreach (var modification in _modifications)
            {
                if(modification.Applies(view))
                {
                    view = modification.Modify(view); // consider if we should add a "?? view;" or just let it fail
                }
            }
            return view;
        }
    }
}