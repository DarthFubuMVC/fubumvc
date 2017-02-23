using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using Xunit;
using Rhino.Mocks;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.Security.Authentication.Saml2
{
    
    public class bottle_configuration_integration_tester
    {
        [Fact]
        public void register_with_basic_authentication_enabled()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<AuthenticationSettings>(_ =>
            {
                _.Enabled = true;
                _.Saml2.Enabled = true;
            });

            var samlCertificateRepository = MockRepository.GenerateMock<ISamlCertificateRepository>();
            samlCertificateRepository.Stub(x => x.AllKnownCertificates()).Return(new SamlCertificate[0]);

            registry.Services.SetServiceIfNone<IPrincipalBuilder>(MockRepository.GenerateMock<IPrincipalBuilder>());
            registry.Services.AddService<ISamlResponseHandler>(MockRepository.GenerateMock<ISamlResponseHandler>());
            registry.Services.SetServiceIfNone<ISamlCertificateRepository>(samlCertificateRepository);


            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();
                var strategies = container.GetAllInstances<IAuthenticationStrategy>();
                strategies.First().ShouldBeOfType<SamlAuthenticationStrategy>();

                strategies.Last().ShouldBeOfType<MembershipAuthentication>();
            }



        }

        [Fact]
        public void register_with_basic_authentication_disabled()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<AuthenticationSettings>(_ =>
            {
                _.Enabled = true;
                _.Saml2.Enabled = true;
            });

            var samlCertificateRepository = MockRepository.GenerateMock<ISamlCertificateRepository>();
            samlCertificateRepository.Stub(x => x.AllKnownCertificates()).Return(new SamlCertificate[0]);
            registry.Services.SetServiceIfNone<ISamlCertificateRepository>(samlCertificateRepository);

            registry.Services.SetServiceIfNone<IPrincipalBuilder>(MockRepository.GenerateMock<IPrincipalBuilder>());
            registry.Services.AddService<ISamlResponseHandler>(MockRepository.GenerateMock<ISamlResponseHandler>());
            


            registry.AlterSettings<AuthenticationSettings>(x => {
                x.MembershipEnabled = MembershipStatus.Disabled;
            });

            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();

                var strategies = container.GetAllInstances<IAuthenticationStrategy>();
                strategies.Single().ShouldBeOfType<SamlAuthenticationStrategy>();
            }

        }

        [Fact]
        public void blows_up_with_no_saml_certificate_repository()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<AuthenticationSettings>(_ =>
            {
                _.Enabled = true;
                _.Saml2.Enabled = true;
            });

            var samlCertificateRepository = MockRepository.GenerateMock<ISamlCertificateRepository>();
            samlCertificateRepository.Stub(r => r.AllKnownCertificates())
                                     .Return(new SamlCertificate[0]);

            registry.Services.SetServiceIfNone<IPrincipalBuilder>(MockRepository.GenerateMock<IPrincipalBuilder>());
            registry.Services.AddService<ISamlResponseHandler>(MockRepository.GenerateMock<ISamlResponseHandler>());
            //registry.Services.SetServiceIfNone<ISamlCertificateRepository>(samlCertificateRepository);




            Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() => {
                                                                registry.ToRuntime();
            });

            

        }

        [Fact]
        public void blows_up_with_no_saml_handlers()
        {
            var registry = new FubuRegistry();

            registry.AlterSettings<AuthenticationSettings>(_ =>
            {
                _.Enabled = true;
                _.Saml2.Enabled = true;
            });

            var samlCertificateRepository = MockRepository.GenerateMock<ISamlCertificateRepository>();
            samlCertificateRepository.Stub(r => r.AllKnownCertificates())
                                     .Return(new SamlCertificate[0]);

            registry.Services.SetServiceIfNone<IPrincipalBuilder>(MockRepository.GenerateMock<IPrincipalBuilder>());
            //registry.Services.AddService<ISamlResponseHandler>(MockRepository.GenerateMock<ISamlResponseHandler>());
            registry.Services.SetServiceIfNone<ISamlCertificateRepository>(samlCertificateRepository);




            Exception<FubuException>.ShouldBeThrownBy(() => {
                registry.ToRuntime();
            });

        }
    }
}