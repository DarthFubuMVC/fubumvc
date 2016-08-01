using System.Collections.Generic;
using FubuCore.Descriptions;

namespace FubuMVC.LightningQueues
{
    public class LightningQueueSettings : DescribesItself
    {
        public LightningQueueSettings()
        {
            DefaultPort = 2020;
            MaxDatabases = 5;
            MapSize = 1024*1024*100;
        }

        public void Describe(Description description)
        {
            description.ShortDescription = "Lightning Queues Configuration";

            description.Properties[nameof(DefaultPort)] = DefaultPort.ToString();
            description.Properties[nameof(MaxDatabases)] = MaxDatabases.ToString();
            description.Properties[nameof(MapSize)] = MapSize.ToString();
        }

        public bool Disabled { get; set; }
        public int DefaultPort { get; set; } 

        /// <summary>
        /// Setting this flag to "true" will disable
        /// the LightningQueues transport if there
        /// are no LightningQueues channels
        /// </summary>
        public bool DisableIfNoChannels { get; set; }

        /// <summary>
        /// The number of databases (queues) allowed for the lmdb storage, default is 5
        /// </summary>
        public int MaxDatabases { get; set; }

        /// <summary>
        /// The maximum map size in bytes for the underlying lmdb storage, default is 100 MB in bytes
        /// </summary>
        public int MapSize { get; set; }
    }
}