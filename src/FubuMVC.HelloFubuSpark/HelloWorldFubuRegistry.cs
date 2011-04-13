using FubuMVC.Core;
using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.Core.Urls;
using FubuMVC.HelloFubuSpark.Controllers.Home;

namespace FubuMVC.HelloFubuSpark
{
    public class HelloWorldFubuRegistry : FubuRegistry
    {
        public HelloWorldFubuRegistry()
        {
            IncludeDiagnostics(true);

            Actions
                .IncludeClassesSuffixedWithController();

            Routes
                .HomeIs<HomeInputModel>()
                .IgnoreControllerNamespaceEntirely()
                .IgnoreMethodSuffix("Command")
                .IgnoreMethodSuffix("Query")
                .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Command"), "POST")
                .ConstrainToHttpMethod(action => action.Method.Name.StartsWith("Query"), "GET");


            Policies.Add<AntiForgeryPolicy>();
            Views.TryToAttachWithDefaultConventions();

            HtmlConvention<SampleHtmlConventions>();

            Services(s => s.ReplaceService<IUrlTemplatePattern, JQueryUrlTemplate>());
        }
    }
}
