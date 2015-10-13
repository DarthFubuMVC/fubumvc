namespace FubuMVC.LightningQueues.Diagnostics
{
    public class ErrorQueueMessageVisualization : QueueMessageVisualization
    {
        public ExceptionDetails ExceptionDetails { get; set; }
    }
}