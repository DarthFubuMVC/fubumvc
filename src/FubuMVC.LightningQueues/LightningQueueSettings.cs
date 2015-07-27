using System;

namespace FubuMVC.LightningQueues
{
    public class LightningQueueSettings
    {
        public LightningQueueSettings()
        {
            DefaultPort = 2020;
            PurgeQueuesPolling = TimeSpan.FromMinutes(3).TotalMilliseconds;

            EnableOutgoingMessageHistory = true;
            EnableProcessedMessageHistory = true;
            NumberOfMessagesToKeepInOutgoingHistory = 100;
            NumberOfMessagesToKeepInProcessedHistory = 100;
            NumberOfReceivedMessageIdsToKeep = 10000;
            OldestMessageInOutgoingHistory = TimeSpan.FromDays(1.0);
            OldestMessageInProcessedHistory = TimeSpan.FromDays(1.0);
        }

        public bool Disabled { get; set; }
        public int DefaultPort { get; set; } 

        /// <summary>
        /// Setting this flag to "true" will disable
        /// the LightningQueues transport if there
        /// are no LightningQueues channels
        /// </summary>
        public bool DisableIfNoChannels { get; set; }

        public double PurgeQueuesPolling { get; set; }

        //Settings to control retention of historical messages
        public bool EnableOutgoingMessageHistory { get; set; }

        public bool EnableProcessedMessageHistory { get; set; }

        public int NumberOfMessagesToKeepInOutgoingHistory { get; set; }

        public int NumberOfMessagesToKeepInProcessedHistory { get; set; }

        public int NumberOfReceivedMessageIdsToKeep { get; set; }

        public TimeSpan OldestMessageInOutgoingHistory { get; set; }

        public TimeSpan OldestMessageInProcessedHistory { get; set; }
    }
}