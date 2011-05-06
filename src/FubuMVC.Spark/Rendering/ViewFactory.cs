using System.Collections.Generic;
using System.Linq;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IViewFactory
    {
        ISparkView GetView();
    }

    public class ViewFactory : IViewFactory
    {
        private readonly IViewEntrySource _viewEntrySource;
        private readonly IEnumerable<ISparkViewModification> _modifications;

        public ViewFactory(IViewEntrySource viewEntrySource, IEnumerable<ISparkViewModification> modifications)
        {
            _modifications = modifications;
            _viewEntrySource = viewEntrySource;
        }

        public ISparkView GetView()
        {
            var view = _viewEntrySource.GetViewEntry().CreateInstance();
            applyModifications(view);
            return view;
        }

        private void applyModifications(ISparkView view)
        {
            _modifications.Where(m => m.Applies(view)).Each(m => m.Modify(view));
        }
    }
}