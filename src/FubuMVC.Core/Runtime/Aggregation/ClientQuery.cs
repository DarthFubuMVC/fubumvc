namespace FubuMVC.Core.Runtime.Aggregation
{
    public class ClientQuery
    {
        public string type { get; set; }
        public object query { get; set; }
        public string correlationId { get; set; }
    }
}
