using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.Services.Messaging.Tracking;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public abstract class MessageLogRecord : LogRecord
    {
        public static readonly string MessageTrackType = "Handler Chain Execution";

        public abstract MessageRecord ToRecord();

        public abstract MessageTrack ToMessageTrack();
    }
}