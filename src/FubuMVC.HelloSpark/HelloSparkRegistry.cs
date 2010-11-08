using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Routes;
using Spark;
using Spark.Web.FubuMVC;
using Spark.Web.FubuMVC.Extensions;
using FubuMVC.HelloSpark.Controllers;
using Spark.Web.FubuMVC.ViewCreation;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.UI;

namespace FubuMVC.HelloSpark
{
    public class HelloSparkRegistry : SparkFubuRegistry
    {
        public HelloSparkRegistry(SparkViewFactory factory)
            : base(factory)
        {
            IncludeDiagnostics(true);

            Applies
                .ToThisAssembly();

            AddViewFolder("/Features/");

            Actions
                .IncludeTypesNamed(x => x.EndsWith("Controller"));

            Routes
                .UrlPolicy<HelloSparkUrlPolicy>();
            
            SparkPolicies
                .AttachViewsBy(call => call.HandlerType.Name.EndsWith("Controller"), 
                                    call => call.HandlerType.Name.RemoveSuffix("Controller"), call => call.Method.Name);
                
            Output.ToJson.WhenTheOutputModelIs<JsonResponse>();
            Output.To(call => new JavaScriptOutputNode(GetJavaScriptViewToken(call), call))
                .WhenTheOutputModelIs<JavaScriptResponse>();

            this.UseDefaultHtmlConventions();

            HomeIs<AirController>(c => c.TakeABreath());
        }

        private SparkViewToken GetJavaScriptViewToken(ActionCall call)
        {
            var response = JavaScriptResponse.GetResponse(call);
            string controllerName = call.HandlerType.Name.RemoveSuffix("Controller");
            return Factory.GetViewToken(call, controllerName, response.ViewName, LanguageType.Javascript);
        }
    }

    public class HelloSparkUrlPolicy : IUrlPolicy
    {
        public bool Matches(ActionCall call, IConfigurationObserver log)
        {
            return call.HandlerType.Name.EndsWith("Controller");
        }

        public IRouteDefinition Build(ActionCall call)
        {
            var route = call.ToRouteDefinition();
            route.Append(call.HandlerType.Name.RemoveSuffix("Controller"));
            route.Append(call.Method.Name);
            return route;
        }
    }
}
