using System;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Logging;

namespace FubuMVC.Core.ServiceBus.Diagnostics
{

    public class MessageRecordListener : ILogListener
    {
        private readonly IMessagingSession _session;

        public MessageRecordListener(IMessagingSession session)
        {
            _session = session;
        }

        public bool ListensFor(Type type)
        {
            return type.CanBeCastTo<MessageLogRecord>();
        }

        public void DebugMessage(object message)
        {
            var record = message.As<MessageLogRecord>().ToRecord();
            if (record.IsPollingJobRelated()) return;

            _session.Record(record);
        }

        public void InfoMessage(object message)
        {
            DebugMessage(message); // same difference here
        }

        public void Debug(string message)
        {
        }

        public void Info(string message)
        {
        }

        public void Error(string message, Exception ex)
        {
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            _session.Record(new MessageRecord
            {
                Id = correlationId.ToString(),
                Message = "Error logged",
                ExceptionText = ex.ToString()
            });
        }

        public bool IsDebugEnabled
        {
            get
            {
                return true;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return true;
            }
        }
    }
}