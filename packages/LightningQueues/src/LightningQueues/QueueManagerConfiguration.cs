using System;

namespace LightningQueues
{
    public class QueueManagerConfiguration
    {
        public QueueManagerConfiguration()
        {
            EnableOutgoingMessageHistory = true;
            EnableProcessedMessageHistory = true;
            NumberOfMessagesToKeepInOutgoingHistory = 100;
            NumberOfMessagesToKeepInProcessedHistory = 100;
            NumberOfReceivedMessageIdsToKeep = 10000;
            OldestMessageInOutgoingHistory = TimeSpan.FromDays(1);
            OldestMessageInProcessedHistory = TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Enable to save sent messages to a history table after they're successfully 
        /// sent.  Defaults to true.
        /// </summary>
        public bool EnableOutgoingMessageHistory { get; set; }

        /// <summary>
        /// Enable to save processed messages to a history table after being processed.
        /// Defaults to true.
        /// </summary>
        public bool EnableProcessedMessageHistory { get; set; }

        /// <summary>
        /// Specifies the minimum number of messages to keep in the outgoing message 
        /// history, if EnableOutgoingMessageHistory is true.  Defaults to 100.
        /// 
        /// NOTE: There could potentially be a far greater number of messages stored in 
        /// the history depending on how many messages are processed within the time frame
        /// specified by OldestMessageInOutgoingHistory.
        /// </summary>
        public int NumberOfMessagesToKeepInOutgoingHistory { get; set; }

        /// <summary>
        /// Specifies the minimum number of messages to keep in the processed message 
        /// history, if EnableProcessedMessageHistory is true.  Defaults to 100.
        /// 
        /// NOTE: There could potentially be a far greater number of messages stored in 
        /// the history depending on how many messages are processed within the time frame
        /// specified by OldestMessageInProcessedHistory.
        /// </summary>
        public int NumberOfMessagesToKeepInProcessedHistory { get; set; }

        /// <summary>
        /// Specifies the number of received message IDs to store.  This prevents 
        /// receiving the same message twice in quick succession.  Defaults to 10000, 
        /// after which the oldest are purged.
        /// </summary>
        public int NumberOfReceivedMessageIdsToKeep { get; set; }

        /// <summary>
        /// Specifies the minimum length of time to keep a sent message in history, 
        /// if EnableOutgoingMessageHistory is true.  Defaults to one day.  This should 
        /// be adjusted depending on the expected number of messages sent and the 
        /// desired size of the ESENT file.
        /// </summary>
        public TimeSpan OldestMessageInOutgoingHistory { get; set; }

        /// <summary>
        /// Specifies the minimum length of time to keep a processed message in history, 
        /// if EnableProcessedMessageHistory is true.  Defaults to one day.  This should 
        /// be adjusted depending on the expected number of messages processed and the 
        /// desired size of the ESENT file.
        /// </summary>
        public TimeSpan OldestMessageInProcessedHistory { get; set; }
    }
}