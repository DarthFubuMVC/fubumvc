namespace FubuMVC.Core.Runtime.Aggregation
{
    public class ClientResponse
    {
        public string type { get; set; }
        public string request { get; set; }
        public object result { get; set; }
        public string correlationId { get; set; }
    }
}
