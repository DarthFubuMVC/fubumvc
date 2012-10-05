namespace ViewEngineIntegrationTesting.ViewEngines.Spark
{
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

    public class RemViewModel
    {
        
    }

    public class EarthViewModel
    {
        public string RawUrl { get; set; }
    }

    public class FireInputModel
    {
        
    }

    public class RemInputModel
    {
        
    }

    public class WaterInputModel
    {
        
    }
}