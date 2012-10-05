namespace ViewEngineIntegrationTesting.ViewEngines.Spark.ThreePassRendering
{
    public class ThreePassEndpoint
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