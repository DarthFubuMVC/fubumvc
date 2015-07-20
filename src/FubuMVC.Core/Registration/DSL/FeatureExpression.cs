using FubuMVC.Core.Security.Authentication;

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
    }
}