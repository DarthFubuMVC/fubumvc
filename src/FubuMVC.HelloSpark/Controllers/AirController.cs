using Spark.Web.FubuMVC.ViewCreation;
namespace FubuMVC.HelloSpark.Controllers
{
    public class AirController
    {
        public AirViewModel TakeABreath()
        {
            return new AirViewModel { Text = "Take a breath?" };
        }

        public JavaScriptResponse BreatheView()
        {
            return new JavaScriptResponse() { ViewName = "_Breathe" };
        }

        public JsonResponse Breathe(AirInputModel model)
        {
            var result = model.TakeABreath
                ? new AirViewModel { Text = "Breathe in!" }
                : new AirViewModel { Text = "Exhale!" };

            return new JsonResponse() { Model = result };
        }
    }

    public class AirInputModel
    {
        public bool TakeABreath { get; set; }
    }

    public class AirViewModel
    {
        public string Text { get; set; }
    }

    public class JsonResponse
    {
        public object Model { get; set; }
    }

}