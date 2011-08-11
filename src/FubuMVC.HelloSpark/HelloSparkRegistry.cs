using FubuMVC.Core;
using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.Core.Urls;
using FubuMVC.HelloSpark.Controllers.Air;
using FubuMVC.Spark;

namespace FubuMVC.HelloSpark
{
    public class HelloSparkRegistry : FubuRegistry
    {
        public HelloSparkRegistry()
        {
            IncludeDiagnostics(true);

            Applies
                .ToThisAssembly();

            Actions
                .IncludeClassesSuffixedWithController();

            ApplyHandlerConventions();

            Routes
                .HomeIs<AirController>(c => c.TakeABreath())
                .IgnoreControllerNamespaceEntirely()
                .IgnoreMethodSuffix("Command")
                .IgnoreMethodSuffix("Query")
                .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Command"), "POST")
                .ConstrainToHttpMethod(action => action.Method.Name.StartsWith("Query"), "GET");
			
            Policies.Add<AntiForgeryPolicy>();
            
            this.UseSpark();
            
            Views
                .TryToAttachWithDefaultConventions()
                .TryToAttachViewsInPackages();

            HtmlConvention<SampleHtmlConventions>();

            Services(s => s.ReplaceService<IUrlTemplatePattern, JQueryUrlTemplate>());
        }
    }
}
