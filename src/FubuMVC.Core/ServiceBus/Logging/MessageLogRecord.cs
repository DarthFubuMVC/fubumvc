using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Diagnostics;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public abstract class MessageLogRecord : LogRecord
    {
        public abstract MessageRecord ToRecord();
    }
}