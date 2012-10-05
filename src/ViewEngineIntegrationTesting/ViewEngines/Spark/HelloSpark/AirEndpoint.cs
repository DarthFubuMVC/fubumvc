using FubuCore;

namespace ViewEngineIntegrationTesting.ViewEngines.Spark.HelloSpark
{
    public class AirEndpoint
    {
        public AirViewModel TakeABreath(AirRequest request)
        {
            return new AirViewModel { Text = "Take a {0} breath?".ToFormat(request.Type) };
        }

        public BreatheViewModel Breathe(AirInputModel model)
        {
            var result = model.TakeABreath
                ? new BreatheViewModel { Text = "Breathe in!" }
                : new BreatheViewModel { Text = "Exhale!" };

            return result;
        }
    }

    public class AirRequest
    {
        public AirRequest()
        {
            Type = "deep";
        }

        public string Type { get; set; }
    }

    public class AirInputModel
    {
        public bool TakeABreath { get; set; }
    }

    public class AirViewModel
    {
        public string Text { get; set; }
    }

    public class BreatheViewModel : AirViewModel
    {
        
    }

}