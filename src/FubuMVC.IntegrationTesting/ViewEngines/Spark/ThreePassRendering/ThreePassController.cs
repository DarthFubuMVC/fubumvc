namespace FubuMVC.IntegrationTesting.ViewEngines.Spark.ThreePassRendering
{
    public class ThreePassController
    {
        public ThreePassModel ThreePassSample(ThreePassModel input)
        {
            return input;
        }
    }

    public class ThreePassModel
    {
        public ThreePassModel()
        {
            Message = "Three Pass Rendering Test!";
        }
        public string Message { get; set; }
    }

}