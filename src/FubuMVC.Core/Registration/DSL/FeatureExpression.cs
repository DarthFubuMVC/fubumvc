using FubuMVC.Core.Localization;
using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.ServiceBus;

namespace FubuMVC.Core.Registration.DSL
{
    public class FeatureExpression
    {
        private readonly FubuRegistry _parent;

        public FeatureExpression(FubuRegistry parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Configure the onboard diagnostics behavior
        /// </summary>
        public Feature<DiagnosticsSettings, TraceLevel> Diagnostics
        {
            get
            {
                return new Feature<DiagnosticsSettings, TraceLevel>(_parent, (settings, level) => settings.TraceLevel = level);
            }
        }

        /// <summary>
        /// Configure and enable the built in authentication features
        /// </summary>
        public Feature<AuthenticationSettings, bool> Authentication
        {
            get
            {
                return new Feature<AuthenticationSettings, bool>(_parent, (settings, enabled) => settings.Enabled = enabled);
            }
        }

        /// <summary>
        /// Configure and enable the built in localization features
        /// </summary>
        public Feature<LocalizationSettings, bool> Localization
        {
            get
            {
                return new Feature<LocalizationSettings, bool>(_parent, (settings, enabled) => settings.Enabled = enabled);
            }
        }

        /// <summary>
        /// Configure and enable the built in anti-forgery functionality
        /// </summary>
        public Feature<AntiForgerySettings, bool> AntiForgery
        {
            get
            {
                return new Feature<AntiForgerySettings, bool>(_parent, (settings, enabled) => settings.Enabled = enabled);
            }
        }

        /// <summary>
        /// Configure and enable the service bus features of FubuMVC
        /// </summary>
        public Feature<TransportSettings, bool> ServiceBus
        {
            get
            {
                return new Feature<TransportSettings, bool>(_parent, (settings, enabled) => settings.Enabled = enabled);
            }
        } 
    }
}