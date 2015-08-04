using System;
using System.Linq;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core.Security.Authentication
{
    public class AuthenticationSettings : IFeatureSettings
    {
        private Func<RoutedChain, bool> _exclusions = c => false;
        private readonly AuthenticationChain _strategies;

        public AuthenticationSettings()
        {
            ExpireInMinutes = 180;
            SlidingExpiration = true;

            MaximumNumberOfFailedAttempts = 3;
            CooloffPeriodInMinutes = 60;
            MembershipEnabled = MembershipStatus.Enabled;

            _strategies = new AuthenticationChain();
        }


        public bool ExcludeDiagnostics { get; set; }


        public bool Enabled { get; set; }

        /// <summary>
        ///     Register and orders the IAuthenticationStrategy's applied to this
        ///     FubuMVC application
        /// </summary>
        public AuthenticationChain Strategies
        {
            get { return _strategies; }
        }

        public Func<RoutedChain, bool> ExcludeChains
        {
            get { return _exclusions; }
            set
            {
                if (value == null)
                {
                    _exclusions = c => false;
                }


                _exclusions = value;
            }
        }

        public MembershipStatus MembershipEnabled { get; set; }


        public bool SlidingExpiration { get; set; }
        public int ExpireInMinutes { get; set; }

        public int MaximumNumberOfFailedAttempts { get; set; }
        public int CooloffPeriodInMinutes { get; set; }

        // *should* only be for testing
        public bool NeverExpires { get; set; }

        public bool ShouldBeExcluded(RoutedChain chain)
        {
            if (chain.Calls.Any(x => x.HasAttribute<NotAuthenticatedAttribute>())) return true;
            if (chain.Calls.Any(x => x.HasAttribute<PassThroughAuthenticationAttribute>())) return true;

            if (ExcludeDiagnostics && chain is DiagnosticChain) return true;

            return _exclusions(chain);
        }

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            if (!Enabled) return;

            registry.Actions.IncludeType<LoginController>();
            registry.Actions.IncludeType<LogoutController>();

            registry.Services.IncludeRegistry<AuthenticationServiceRegistry>();

            registry.Policies.Global.Add(new ApplyAuthenticationPolicy());

            if (MembershipEnabled == MembershipStatus.Enabled)
            {
                if (!Strategies.OfType<MembershipNode>().Any())
                {
                    Strategies.InsertFirst(new MembershipNode());
                }
            }

            foreach (IContainerModel strategy in Strategies)
            {
                var instance = strategy.ToInstance();

                registry.Services.AddService(typeof (IAuthenticationStrategy), instance);
            }
        }
    }

    public enum MembershipStatus
    {
        Enabled,
        Disabled
    }
}