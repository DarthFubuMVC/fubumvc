namespace AspNetApplication.ServerSideEvents
{
    public class Topic
    {
        [HeaderValue("Last-Event-ID")]
        public string LastEventId { get; set; }
    }
}