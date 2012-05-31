namespace FubuMVC.Core.Registration
{
    public class EndpointActionSource : ActionSource
    {
        public EndpointActionSource() : base(new ActionMethodFilter())
        {
            TypeFilters.Includes += t => t.Name.EndsWith("Endpoint") || t.Name.EndsWith("Endpoints");
        }
    }
}