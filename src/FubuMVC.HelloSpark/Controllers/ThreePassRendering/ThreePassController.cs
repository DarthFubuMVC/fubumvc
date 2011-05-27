namespace FubuMVC.HelloSpark.Controllers.ThreePassRendering
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
        public string Message { get; set; }
        public ThreePassModel()
        {
            Message = "Three Pass Rendering Test!";
        }
    }

}