using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace FubuMVC.AspNet
{
    public class Loader
    {
        public static void LoadModule()
        {
            DynamicModuleUtility.RegisterModule(typeof (FubuAspNetHost));
        }
    }
}