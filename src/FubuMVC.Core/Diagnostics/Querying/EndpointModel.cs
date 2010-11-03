namespace FubuMVC.Core.Diagnostics.Querying
{
    public class EndpointModel : JsonMessage
    {
        public EndpointToken[] AllEndpoints { get; set; }
    }
}