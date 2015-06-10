using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration;

namespace FubuMVC.Nowin
{
    [ApplicationLevel, Serializable]
    [Title("Katana Auto Hosting Settings")]
    public class NowinSettings
    {
        public NowinSettings()
        {
            Port = 5500;
        }

        public int Port { get; set; }
        public bool AutoHostingEnabled { get; set; }
        internal EmbeddedFubuMvcServer EmbeddedServer { get; set; }
    }
}
