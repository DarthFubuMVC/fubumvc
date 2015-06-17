using System.Collections.Generic;

namespace FubuTransportation.Diagnostics
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