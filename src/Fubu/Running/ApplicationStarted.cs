using System;
using FubuMVC.Core;
using FubuMVC.Core.Assets;

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