using System;
using FubuCore.Descriptions;
using HtmlTags;

namespace FubuMVC.LightningQueues
{
    public class LightningQueueSettings : DescribesItself
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

        public void Describe(Description description)
        {
            description.ShortDescription = "Lightning Queues Configuration";

            description.Properties[nameof(DefaultPort)] = DefaultPort.ToString();
            description.Properties[nameof(PurgeQueuesPolling)] = PurgeQueuesPolling.ToString();
            description.Properties[nameof(EnableOutgoingMessageHistory)] = EnableOutgoingMessageHistory.ToString();
            description.Properties[nameof(EnableProcessedMessageHistory)] = EnableProcessedMessageHistory.ToString();
            description.Properties[nameof(NumberOfMessagesToKeepInOutgoingHistory)] = NumberOfMessagesToKeepInOutgoingHistory.ToString();
            description.Properties[nameof(NumberOfReceivedMessageIdsToKeep)] = NumberOfReceivedMessageIdsToKeep.ToString();
            description.Properties[nameof(OldestMessageInOutgoingHistory)] = OldestMessageInOutgoingHistory.ToString();
            description.Properties[nameof(OldestMessageInProcessedHistory)] = OldestMessageInProcessedHistory.ToString();
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