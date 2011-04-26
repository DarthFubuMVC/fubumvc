using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using Microsoft.Practices.ServiceLocation;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface ISparkViewModification
    {
        bool Applies(ISparkView view);
        void Modify(ISparkView view);
    }

    class ModelAttacher : ISparkViewModification
    {
        private readonly IFubuRequest _fubuRequest;
        public ModelAttacher(IFubuRequest fubuRequest)
        {
            _fubuRequest = fubuRequest;
        }

        public bool Applies(ISparkView view)
        {
            return view is IFubuViewWithModel;
        }

        public void Modify(ISparkView view)
        {
            ((IFubuViewWithModel)view).SetModel(_fubuRequest);
        }
    }

    public class ServiceLocatorAttacher : ISparkViewModification
    {
        private readonly IServiceLocator _serviceLocator;
        public ServiceLocatorAttacher(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public bool Applies(ISparkView view)
        {
            return view is IFubuPage;
        }

        public void Modify(ISparkView view)
        {
            ((IFubuPage)view).ServiceLocator = _serviceLocator;
        }
    }
}