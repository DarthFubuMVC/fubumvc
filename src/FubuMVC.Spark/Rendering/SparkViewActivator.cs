using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using Microsoft.Practices.ServiceLocation;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    // We could generalize this to ISparkViewModification strategies (makes easy for pluggability)
    public interface ISparkViewActivator
    {
        void Activate(ISparkView sparkView);
    }

    public class SparkViewActivator : ISparkViewActivator
    {
        private readonly IServiceLocator _locator;
        private readonly IFubuRequest _request;

        public SparkViewActivator(IServiceLocator locator, IFubuRequest request)
        {
            _locator = locator;
            _request = request;
        }

        public void Activate(ISparkView sparkView)
        {
            if (sparkView is IFubuPage)
            {
                ((IFubuPage)sparkView).ServiceLocator = _locator;
            }

            if (sparkView is IFubuViewWithModel)
            {
                ((IFubuViewWithModel)sparkView).SetModel(_request);
            }
        }
    }
}