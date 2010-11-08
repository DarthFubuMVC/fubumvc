using FubuMVC.Core;
using FubuMVC.HelloWorld.Controllers.Home;
using FubuMVC.HelloWorld.Controllers.Products;
using FubuMVC.HelloWorld.Services;
using FubuMVC.StructureMap;
using FubuMVC.StructureMap.Bootstrap;
using FubuMVC.UI;
using FubuValidation;
using FubuValidation.Registration;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace FubuMVC.HelloWorld
{
    public class Global : FubuStructureMapApplication
    {
        protected override void InitializeValidation()
        {
            Validator.Initialize<HelloWorldValidationRegistry>();
            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(ObjectFactory.Container));
        }

        public override FubuRegistry GetMyRegistry()
        {
            return new HelloWorldFubuRegistry();
        }

        protected override void InitializeStructureMap(IInitializationExpression ex)
        {
            ex.For<IHttpSession>().Use<CurrentHttpContextSession>();
            ex.For<IValidationProvider>().Use(() => Validator.Provider);
            ex.For<IValidationQuery>().Use(() => Validator.Model);
        }
    }

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