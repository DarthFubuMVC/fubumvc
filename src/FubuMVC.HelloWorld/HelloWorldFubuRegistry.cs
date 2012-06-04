using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.Core.UI.Extensibility;
using FubuMVC.Core.Urls;
using FubuMVC.HelloWorld.Controllers.Air;
using FubuMVC.HelloWorld.Controllers.Home;
using FubuMVC.HelloWorld.Controllers.OutputModels;
using FubuMVC.HelloWorld.Controllers.Products;
using FubuMVC.Razor;
using FubuMVC.Spark;
using FubuMVC.WebForms;

namespace FubuMVC.HelloWorld
{
    public class something
    {
        public void go()
        {
            Debug.WriteLine(new HelloWorldFubuRegistry());
        }
    }

    public class HelloWorldFubuRegistry : FubuRegistry
    {
        public HelloWorldFubuRegistry()
        {
            Actions
                .IncludeClassesSuffixedWithController();

            Routes
                .HomeIs<HomeInputModel>()
                .IgnoreControllerNamespaceEntirely()
                .IgnoreMethodSuffix("Command")
                .IgnoreMethodSuffix("Query")
                .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Command"), "POST")
                .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Post"), "POST")
                .ConstrainToHttpMethod(action => action.Method.Name.StartsWith("Query"), "GET");

            ApplyConvention<OutputModelSettingBehaviorConfiguration>();

            Policies
                .Add<AntiForgeryPolicy>();

            Import<WebFormsEngine>();

            Views
                .TryToAttachWithDefaultConventions();

            HtmlConvention<SampleHtmlConventions>();

            this.RegisterPartials(x => x.For<Product>().Use<ProductPartial>());
            this.RegisterPartials(x => x.For<ProductPart>().Use<PartPartial>());


            Services(s => s.ReplaceService<IUrlTemplatePattern, JQueryUrlTemplate>());


            Services(s =>
            {
                s.FillType<IExceptionHandler, AsyncExceptionHandler>();
                s.ReplaceService<IUrlTemplatePattern, JQueryUrlTemplate>();
            });

            this.Extensions()
                .For<AirViewModel>("extension-placeholder", x => "<p>Rendered from content extension.</p>");
        }
    }
}
