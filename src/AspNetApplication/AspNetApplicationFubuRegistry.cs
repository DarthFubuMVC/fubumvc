using AspNetApplication.ServerSideEvents;
using FubuMVC.Core;

namespace AspNetApplication
{
    public class AspNetApplicationFubuRegistry : FubuRegistry
    {
        public AspNetApplicationFubuRegistry()
        {
            Actions.IncludeClassesSuffixedWithController();

            Views.TryToAttachWithDefaultConventions();
            Routes.HomeIs<SSEClientController>(x => x.get_events());
        }
    }
}