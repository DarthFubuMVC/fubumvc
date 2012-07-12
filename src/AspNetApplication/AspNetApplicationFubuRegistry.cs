using FubuMVC.Core;

namespace AspNetApplication
{
    public class AspNetApplicationFubuRegistry : FubuRegistry
    {
        public AspNetApplicationFubuRegistry()
        {
            Actions.IncludeClassesSuffixedWithController();

            Views.TryToAttachWithDefaultConventions();
        }
    }
}