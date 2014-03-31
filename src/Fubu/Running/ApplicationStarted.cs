using System;

namespace Fubu.Running
{
    public class ApplicationStarted
    {
        public string[] BottleContentFolders { get; set; }
        public string HomeAddress { get; set; }
        public string ApplicationName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}