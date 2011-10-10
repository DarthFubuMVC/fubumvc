using FubuMVC.Core;

namespace FubuMVC.HelloWorld.Controllers.Conditional
{
    public class ConditionalController
    {
        public ConditionalModel Go(ConditionalRequest request)
        {
            return new ConditionalModel();
        }
    }

    public class ConditionalRequest
    {
        [QueryString]
        public bool render { get; set; }
    }

    public class ConditionalModel
    {
    }
}