using System;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core.Security.Authentication
{
    public class AuthenticationSettings : IFeatureSettings, DescribesItself
    {
        private Func<RoutedChain, bool> _exclusions = c => false;

        public AuthenticationSettings()
        {
            ExpireInMinutes = 180;
            SlidingExpiration = true;

            MaximumNumberOfFailedAttempts = 3;
            CooloffPeriodInMinutes = 60;
            MembershipEnabled = MembershipStatus.Enabled;

            Strategies = new AuthenticationChain();
        }

        public void Describe(Description description)
        {
            description.ShortDescription = "Authentication Configuration for the Running Application";
            description.Properties[nameof(Enabled)] = Enabled.ToString();
            description.Properties[nameof(ExcludeDiagnostics)] = ExcludeDiagnostics.ToString();
            description.Properties[nameof(ExpireInMinutes)] = ExpireInMinutes.ToString();
            description.Properties[nameof(SlidingExpiration)] = SlidingExpiration.ToString();
            description.Properties[nameof(MaximumNumberOfFailedAttempts)] = MaximumNumberOfFailedAttempts.ToString();
            description.Properties[nameof(MembershipEnabled)] = MembershipEnabled.ToString();
            description.Properties[nameof(NeverExpires)] = NeverExpires.ToString();

            description.AddChild("Saml2", Saml2);

            description.AddList("Strategies", Strategies);
        }

        public Saml Saml2 { get; set; } = new Saml();


        public bool ExcludeDiagnostics { get; set; }


        public bool Enabled { get; set; }

        /// <summary>
        ///     Register and orders the IAuthenticationStrategy's applied to this
        ///     FubuMVC application
        /// </summary>
        public AuthenticationChain Strategies { get; }

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

            if (Saml2.Enabled)
            {
                Strategies.InsertFirst(new AuthenticationNode(typeof(SamlAuthenticationStrategy)));
                registry.Services.IncludeRegistry<Saml2ServicesRegistry>();


                if (Saml2.RequireSignature)
                {
                    registry.Services.AddService<ISamlValidationRule, SignatureIsRequired>();
                }

                if (Saml2.RequireCertificate)
                {
                    registry.Services.AddService<ISamlValidationRule, CertificateValidation>();
                }

                if (Saml2.EnforceConditionalTimeSpan)
                {
                    registry.Services.AddService<ISamlValidationRule, ConditionTimeFrame>();
                }
            }

            foreach (IContainerModel strategy in Strategies)
            {
                var instance = strategy.ToInstance();

                registry.Services.AddService(typeof (IAuthenticationStrategy), instance);
            }


        }

        public class Saml : DescribesItself
        {
            public Saml()
            {
                RequireCertificate = true;
                RequireSignature = true;
                EnforceConditionalTimeSpan = true;
            }

            public bool RequireSignature { get; set; }
            public bool RequireCertificate { get; set; }
            public bool EnforceConditionalTimeSpan { get; set; }

            public bool Enabled { get; set; }
            public void Describe(Description description)
            {
                description.ShortDescription = "Saml2 Configuration";
                description.Properties[nameof(Enabled)] = Enabled.ToString();
                description.Properties[nameof(RequireCertificate)] = RequireCertificate.ToString();
                description.Properties[nameof(RequireSignature)] = RequireSignature.ToString();
                description.Properties[nameof(EnforceConditionalTimeSpan)] = EnforceConditionalTimeSpan.ToString();
            }
        }


    }



    public enum MembershipStatus
    {
        Enabled,
        Disabled
    }
}