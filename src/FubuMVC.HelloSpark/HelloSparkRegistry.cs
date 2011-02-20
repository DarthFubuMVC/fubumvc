using FubuMVC.Core;
using FubuMVC.HelloSpark.Controllers;
using FubuMVC.Validation;
using FubuValidation;
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

            HomeIs<AirController>(c => c.TakeABreath());

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

            this.Validation<ValidationRegistry>(validation => validation
                                                                  .Failures
                                                                  .IfModelTypeIs<CreateProductInput>()
                                                                  .TransferTo<CreateProductRequest>());

            Routes
                .UrlPolicy<HelloSparkUrlPolicy>();
            
            Output
				.ToJson
				.WhenTheOutputModelIs<JsonResponse>();
        }
    }
}
