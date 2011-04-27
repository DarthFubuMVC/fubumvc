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
        private readonly IViewEngine _viewEngine;
        private readonly IEnumerable<ISparkViewModification> _modifications;

        public ViewFactory(IViewEngine viewEngine, IEnumerable<ISparkViewModification> modifications)
        {
            _modifications = modifications;
            _viewEngine = viewEngine;
        }

        public ISparkView GetView()
        {
            var view = _viewEngine.GetViewEntry().CreateInstance();
            applyModifications(view);
            return view;
        }

        private void applyModifications(ISparkView view)
        {
            _modifications
                .Where(m => m.Applies(view)).Each(m => m.Modify(view));
        }
    }
}