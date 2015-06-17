using FubuCore.Logging;
using FubuTransportation.Diagnostics;

namespace FubuTransportation.Logging
{
    public abstract class MessageLogRecord : LogRecord
    {
        public abstract MessageRecord ToRecord();
    }
}