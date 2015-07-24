using FubuCore.Logging;

namespace FubuMVC.RavenDb.Reset
{
    public class ResetMessage : LogTopic
    {
        public string Message { get; set; }
    }
}