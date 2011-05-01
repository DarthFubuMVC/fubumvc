using FubuMVC.Core.Runtime;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View.Activation
{
    public class SetPageModelActivationAction<T> : IPageActivationAction where T : class
    {
        public void Activate(IServiceLocator services, IFubuPage page)
        {
            var modelPage = (IFubuPage<T>)page;
            var request = services.GetInstance<IFubuRequest>();
            modelPage.Model = request.Get<T>();
        }
    }
}