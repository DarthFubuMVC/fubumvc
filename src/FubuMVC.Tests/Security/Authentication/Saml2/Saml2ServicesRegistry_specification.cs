using FubuMVC.Core;
using FubuMVC.Core.Navigation;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using FubuMVC.Core.Security.Authentication.Saml2.Encryption;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.Security.Authentication.Saml2
{
    [TestFixture]
    public class Saml2ServicesRegistry_specification
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            var registry = new FubuRegistry();
            registry.Features.Authentication.Configure(x =>
            {
                x.Enabled = true;
                x.Saml2.Enabled = true;
            });

            registry.Services.AddService<IPrincipalBuilder>(MockRepository.GenerateMock<IPrincipalBuilder>());
            var samlCertificateRepository = MockRepository.GenerateMock<ISamlCertificateRepository>();
            samlCertificateRepository.Stub(x => x.AllKnownCertificates()).Return(new SamlCertificate[0]);
            registry.Services.SetServiceIfNone<ISamlCertificateRepository>(samlCertificateRepository);
            registry.Services.SetServiceIfNone<IPrincipalBuilder>(MockRepository.GenerateMock<IPrincipalBuilder>());
            registry.Services.AddService<ISamlResponseHandler>(MockRepository.GenerateMock<ISamlResponseHandler>());

            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();

                container.Model.For<TService>().Default.ReturnedType.ShouldBe(typeof(TImplementation));
            }

        }

        [Test]
        public void IAssertionXmlEncryptor()
        {
            registeredTypeIs<IAssertionXmlEncryptor, AssertionXmlEncryptor>();
        }

        [Test]
        public void ISamlResponseXmlSigner()
        {
            registeredTypeIs<ISamlResponseXmlSigner, SamlResponseXmlSigner>();
        }


        [Test]
        public void ISamlDirector()
        {
            registeredTypeIs<ISamlDirector, SamlDirector>();
        }

        [Test]
        public void ISamlResponseReader()
        {
            registeredTypeIs<ISamlResponseReader, SamlResponseReader>();
        }

        [Test]
        public void ISamlResponseRedirector()
        {
            registeredTypeIs<ISamlResponseRedirector, SamlResponseRedirector>();
        }

        [Test]
        public void ISamlResponseWriter()
        {
            registeredTypeIs<ISamlResponseWriter, SamlResponseWriter>();
        }

        [Test]
        public void ICertificateService()
        {
            registeredTypeIs<ICertificateService, CertificateService>();
        }

        [Test]
        public void ICertificateLoader()
        {
            registeredTypeIs<ICertificateLoader, CertificateLoader>();
        }

        [Test]
        public void IAssertionXmlDecryptor()
        {
            registeredTypeIs<IAssertionXmlDecryptor, AssertionXmlDecryptor>();
        }
    }
}