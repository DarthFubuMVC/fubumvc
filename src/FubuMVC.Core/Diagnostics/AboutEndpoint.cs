using System;

namespace FubuMVC.Core.Diagnostics
{
    public class AboutEndpoint
    {
        private readonly AppReloaded _reloaded;

        public AboutEndpoint(AppReloaded reloaded)
        {
            _reloaded = reloaded;
        }

        public string get__about()
        {
            return FubuApplicationDescriber.WriteDescription();
        }

        public string get__loaded()
        {
            return _reloaded.Timestamp.ToString();
        }
    }

    public class AppReloaded
    {
        public DateTime Timestamp = DateTime.UtcNow;
    }
}