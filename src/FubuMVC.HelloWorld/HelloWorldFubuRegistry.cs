using FubuMVC.Core;
using FubuMVC.HelloWorld.Controllers.Home;
using FubuMVC.HelloWorld.Controllers.Products;
using FubuMVC.UI;

namespace FubuMVC.HelloWorld
{
    public class HelloWorldFubuRegistry : FubuRegistry
    {
        public HelloWorldFubuRegistry()
        {
            IncludeDiagnostics(true);

            Applies.ToThisAssembly();

            Actions
                .IncludeTypesNamed(x => x.EndsWith("Controller"));

            Routes
                .IgnoreControllerNamespaceEntirely()
                .IgnoreMethodSuffix("Command")
                .IgnoreMethodSuffix("Query")
                .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Command"), "POST")
                .ConstrainToHttpMethod(action => action.Method.Name.StartsWith("Query"), "GET");

            Views
                .TryToAttach(x=>
                                 {
                                     x.by_ViewModel_and_Namespace_and_MethodName();
                                     x.by_ViewModel_and_Namespace();
                                     x.by_ViewModel();
                                 });

            
            this.HtmlConvention<SampleHtmlConventions>();

            RegisterPartials(x => x.For<Product>().Use<ProductPartial>());

            HomeIs<HomeInputModel>();
        }
    }
}