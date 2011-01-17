using FubuMVC.Core;
using FubuMVC.HelloWorld.Controllers.Home;
using FubuMVC.HelloWorld.Controllers.Products;

namespace FubuMVC.HelloWorld
{
    public class HelloWorldFubuRegistry : FubuRegistry
    {
        public HelloWorldFubuRegistry()
        {
            IncludeDiagnostics(true);

            HomeIs<HomeInputModel>();

            Actions
                .IncludeClassesSuffixedWithController();

            Routes
                .IgnoreControllerNamespaceEntirely()
                .IgnoreMethodSuffix("Command")
                .IgnoreMethodSuffix("Query")
                .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Command"), "POST")
                .ConstrainToHttpMethod(action => action.Method.Name.StartsWith("Query"), "GET");


            Views.TryToAttachWithDefaultConventions();
            
            HtmlConvention<SampleHtmlConventions>();

            RegisterPartials(x => x.For<Product>().Use<ProductPartial>());
        }
    }
}