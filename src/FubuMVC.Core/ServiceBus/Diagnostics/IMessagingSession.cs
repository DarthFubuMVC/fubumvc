using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.Diagnostics
{
    public interface IMessagingSession
    {
        void ClearAll();
        void Record(MessageRecord record);
        IEnumerable<MessageHistory> TopLevelMessages();
        IEnumerable<MessageHistory> AllMessages();

        IEnumerable<MessageRecord> All();
    }
}