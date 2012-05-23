namespace AspNetApplication.ServerSideEvents
{
    // TODO -- should ServerEvent require Id?
    public class ServerEvent
    {
        public string Data { get; set; }
        public string Id { get; set; }
        public string Event { get; set; }
        public int? Retry { get; set; }
    }
}