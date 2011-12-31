using FubuMVC.Core;
using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.HelloRazor.Controllers.Home;
using FubuMVC.Razor;

namespace FubuMVC.HelloRazor
{
    public class HelloRazorRegistry : FubuRegistry
    {
        public HelloRazorRegistry()
        {
            IncludeDiagnostics(true);

            Applies
                .ToThisAssembly();

            Actions
                .IncludeClassesSuffixedWithController();

            ApplyHandlerConventions();

            Routes
                .HomeIs<HelloWorldInputModel>()
                .IgnoreControllerNamespaceEntirely()
                .IgnoreMethodSuffix("Command")
                .IgnoreMethodSuffix("Query")
                .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Command"), "POST")
                .ConstrainToHttpMethod(action => action.Method.Name.StartsWith("Query"), "GET");
			
            Policies.Add<AntiForgeryPolicy>();
            
            this.UseRazor();
            
            Views
                .TryToAttachWithDefaultConventions()
                .TryToAttachViewsInPackages();

        }
    }
}
