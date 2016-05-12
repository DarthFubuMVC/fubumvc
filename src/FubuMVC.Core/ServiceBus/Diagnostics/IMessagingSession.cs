using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.Diagnostics
{
    public interface IMessagingSession
    {
        void ClearAll();
        void Record(MessageRecord record);
        IEnumerable<MessageLog> TopLevelMessages();
        IEnumerable<MessageLog> AllMessages();
        IEnumerable<MessageRecord> All();
    }
}