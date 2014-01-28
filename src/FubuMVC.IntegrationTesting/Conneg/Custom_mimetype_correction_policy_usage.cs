using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Katana;
using FubuMVC.OwinHost;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class Custom_mimetype_correction_policy_usage
    {
        [Test]
        public void use_the_custom_mimetype_correction()
        {
            using (var server = FubuApplication.For<RegistryWithCustomMimetypeCorrection>().StructureMap().RunEmbedded(port:PortFinder.FindPort(5502)))
            {
                server.Endpoints.GetByInput(new OverriddenResponse{Name="Foo"}, acceptType: "text/html")
                    .ContentTypeShouldBe(MimeType.Json);
            }
        }
    }

    public class RegistryWithCustomMimetypeCorrection : FubuRegistry
    {
        public RegistryWithCustomMimetypeCorrection()
        {
            AlterSettings<ConnegSettings>(x => {
                x.Corrections.Add(new AlwaysJson());
            });
        }
    }

    

    public class AlwaysJson : IMimetypeCorrection
    {
        public void Correct(CurrentMimeType mimeType, ICurrentHttpRequest request, BehaviorChain chain)
        {
            mimeType.AcceptTypes = new MimeTypeList(MimeType.Json);
        }
    }
}