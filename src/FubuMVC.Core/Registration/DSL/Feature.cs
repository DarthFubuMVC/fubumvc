using System;

namespace FubuMVC.Core.Registration.DSL
{
    public class Feature<TSettings, TEnabled> where TSettings : class, new()
    {
        protected readonly FubuRegistry _parent;
        private readonly Action<TSettings, TEnabled> _onEnabled;

        public Feature(FubuRegistry parent, Action<TSettings, TEnabled> onEnabled)
        {
            _parent = parent;
            _onEnabled = onEnabled;
        }

        public void Enable(TEnabled enabled)
        {
            _parent.AlterSettings<TSettings>(settings => _onEnabled(settings, enabled));
        }

        public void Configure(Action<TSettings> configure)
        {
            _parent.AlterSettings(configure);
        }
    }
}