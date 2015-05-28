using FubuCore.Logging;

namespace FubuPersistence.Reset
{
    public class ResetMessage : LogTopic
    {
        public string Message { get; set; }
    }
}