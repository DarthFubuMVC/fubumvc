using FubuMVC.Core;
using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.Core.Urls;
using FubuMVC.HelloWorld.Controllers.Home;
using FubuMVC.HelloWorld.Controllers.OutputModels;
using FubuMVC.HelloWorld.Controllers.Products;
using FubuMVC.WebForms;

namespace FubuMVC.HelloWorld
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

            ApplyConvention<OutputModelSettingBehaviorConfiguration>();

            Policies.Add<AntiForgeryPolicy>();
            Views.TryToAttachWithDefaultConventions();

            HtmlConvention<SampleHtmlConventions>();

            this.RegisterPartials(x => x.For<Product>().Use<ProductPartial>());
            this.RegisterPartials(x => x.For<ProductPart>().Use<PartPartial>());

            Services(s => s.ReplaceService<IUrlTemplatePattern, JQueryUrlTemplate>());
        }
    }
}
