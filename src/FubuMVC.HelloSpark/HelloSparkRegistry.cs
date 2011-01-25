using FubuMVC.Core;
using FubuMVC.HelloSpark.Controllers;

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
                .IncludeTypesNamed(x => x.EndsWith("Controller"));

            Routes
                .UrlPolicy<HelloSparkUrlPolicy>();
            
            Output
				.ToJson
				.WhenTheOutputModelIs<JsonResponse>();

            HomeIs<AirController>(c => c.TakeABreath());
        }
    }
}
