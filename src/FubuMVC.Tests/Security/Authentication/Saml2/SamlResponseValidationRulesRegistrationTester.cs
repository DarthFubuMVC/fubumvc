using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;
using Xunit;
using Rhino.Mocks;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.Security.Authentication.Saml2
{
    
    public class SamlResponseValidationRulesRegistrationTester
    {
        public SamlResponseValidationRulesRegistrationTester()
        {
            LocalizationManager.Stub();
        }

        [Fact]
        public void require_signatures_is_true()
        {
            using (var runtime = FubuRuntime.Basic(_ =>
            {
                enableSaml2(_);
                _.Features.Authentication.Configure(x =>
                {
                    x.Saml2.RequireSignature = true;
                });

            }))
            {
                runtime.Get<IContainer>().Model.For<ISamlValidationRule>()
                    .Instances
                    .Select(x => x.ReturnedType)
                    .ShouldContain(typeof(SignatureIsRequired));
            }
        }

        [Fact]
        public void require_signatures_is_false()
        {
            using (var runtime = FubuRuntime.Basic(_ =>
            {
                enableSaml2(_);
                _.Features.Authentication.Configure(x =>
                {
                    x.Saml2.RequireSignature = false;
                });
            }))
            {
                runtime.Get<IContainer>().Model.For<ISamlValidationRule>()
                    .Instances
                    .Select(x => x.ReturnedType)
                    .ShouldNotContain(typeof(SignatureIsRequired));
            }

        }

        [Fact]
        public void require_certificate_is_true()
        {
            using (var runtime = FubuRuntime.Basic(_ =>
            {
                enableSaml2(_);
                _.Features.Authentication.Configure(x =>
                {
                    x.Saml2.RequireCertificate = true;
                });

            }))
            {
                runtime.Get<IContainer>().Model.For<ISamlValidationRule>()
                    .Instances
                    .Select(x => x.ReturnedType)
                    .ShouldContain(typeof(CertificateValidation));
            }
        }

        [Fact]
        public void require_certificate_is_false()
        {
            using (var runtime = FubuRuntime.Basic(_ =>
            {
                enableSaml2(_);
                _.Features.Authentication.Configure(x =>
                {
                    x.Saml2.RequireCertificate = false;
                });
            }))
            {
                runtime.Get<IContainer>().Model.For<ISamlValidationRule>()
                    .Instances
                    .Select(x => x.ReturnedType)
                    .ShouldNotContain(typeof(CertificateValidation));
            }

        }

        [Fact]
        public void enforce_response_timespan_is_true()
        {
            using (var runtime = FubuRuntime.Basic(_ =>
            {
                enableSaml2(_);

                _.Features.Authentication.Configure(x =>
                {
                    x.Saml2.EnforceConditionalTimeSpan = true;
                });
            }))
            {
                runtime.Get<IContainer>().Model.For<ISamlValidationRule>()
                    .Instances
                    .Select(x => x.ReturnedType)
                    .ShouldContain(typeof(ConditionTimeFrame));
            }
        }

        private static void enableSaml2(FubuRegistry _)
        {
            _.Features.Authentication.Configure(x =>
            {
                x.Enabled = true;
                x.Saml2.Enabled = true;
            });

            var samlCertificateRepository = MockRepository.GenerateMock<ISamlCertificateRepository>();
            samlCertificateRepository.Stub(x => x.AllKnownCertificates()).Return(new SamlCertificate[0]);

            _.Services.SetServiceIfNone<IPrincipalBuilder>(MockRepository.GenerateMock<IPrincipalBuilder>());
            _.Services.AddService<ISamlResponseHandler>(MockRepository.GenerateMock<ISamlResponseHandler>());
            _.Services.SetServiceIfNone<ISamlCertificateRepository>(samlCertificateRepository);
        }

        [Fact]
        public void require_conditional_timespan_is_false()
        {
            using (var runtime = FubuRuntime.Basic(_ =>
            {
                enableSaml2(_);
                _.Features.Authentication.Configure(x =>
                {
                    x.Saml2.EnforceConditionalTimeSpan = false;
                });
            }))
            {
                runtime.Get<IContainer>().Model.For<ISamlValidationRule>()
                    .Instances
                    .Select(x => x.ReturnedType)
                    .ShouldNotContain(typeof(ConditionTimeFrame));
            }
        }
    }
}