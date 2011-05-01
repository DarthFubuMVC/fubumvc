using FubuMVC.Core.UI;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View.Activation
{
    public class SetTagProfilePageActivationAction<T> : IPageActivationAction where T : class
    {
        private readonly string _profile;

        public SetTagProfilePageActivationAction(string profile)
        {
            _profile = profile;
        }

        public void Activate(IServiceLocator services, IFubuPage page)
        {
            var modelPage = (IFubuPage<T>)page;
            modelPage.Tags().SetProfile(_profile);
        }
    }
}