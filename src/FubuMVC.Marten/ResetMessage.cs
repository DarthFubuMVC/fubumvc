using FubuCore.Logging;

namespace FubuMVC.Marten
{
    public class ResetMessage : LogTopic
    {
        public string Message { get; set; }
    }
}