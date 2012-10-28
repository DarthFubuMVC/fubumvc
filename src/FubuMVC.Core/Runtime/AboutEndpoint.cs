using FubuCore.Descriptions;

namespace FubuMVC.Core.Runtime
{
    public class AboutEndpoint
    {
        public string get__about()
        {
            return FubuApplicationDescriber.WriteDescription();
        }


    }
}