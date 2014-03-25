namespace FubuMVC.Core.Registration
{
    public class EndpointActionSource : ActionSource
    {
        public EndpointActionSource()
        {
            IncludeClassesSuffixedWithEndpoint();
        }
    }
}