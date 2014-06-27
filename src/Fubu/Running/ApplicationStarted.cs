using System;
using FubuMVC.Core;

namespace Fubu.Running
{
    public class ApplicationStarted
    {
        public FileWatcherManifest Watcher { get; set; }
        public string HomeAddress { get; set; }
        public string ApplicationName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}