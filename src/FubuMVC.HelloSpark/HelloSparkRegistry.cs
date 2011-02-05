using FubuMVC.Core;
using FubuMVC.HelloSpark.Controllers;
using Spark.Web.FubuMVC;
using Spark.Web.FubuMVC.ViewCreation;

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

        	this.Spark(spark =>
        	           	{
        	           		spark
        	           			.Settings
        	           			.AddViewFolder("/Features/");

        	           		spark
        	           			.Policies
        	           			.Add<HelloSparkJavaScriptViewPolicy>()
        	           			.Add<HelloSparkPolicy>();

        	           		spark
        	           			.Output
        	           			.ToJavaScriptWhen(call => call.HasOutput && call.OutputType().Equals(typeof (JavaScriptResponse)));

        	           	});

            Routes
                .UrlPolicy<HelloSparkUrlPolicy>();
            
            Output
				.ToJson
				.WhenTheOutputModelIs<JsonResponse>();

            HomeIs<AirController>(c => c.TakeABreath());
        }
    }
}
