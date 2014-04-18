using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class Custom_mimetype_correction_policy_usage
    {
        [Test]
        public void use_the_custom_mimetype_correction()
        {
            TestHost.Scenario<RegistryWithCustomMimetypeCorrection>(_ => {
                _.Get.Input(new OverriddenResponse {Name = "Foo"});
                _.Request.Accepts("text/html");

                _.ContentTypeShouldBe(MimeType.Json);
            });
        }
    }

    public class RegistryWithCustomMimetypeCorrection : FubuRegistry
    {
        public RegistryWithCustomMimetypeCorrection()
        {
            AlterSettings<ConnegSettings>(x => { x.Corrections.Add(new AlwaysJson()); });
        }
    }


    public class AlwaysJson : IMimetypeCorrection
    {
        public void Correct(CurrentMimeType mimeType, IHttpRequest request, BehaviorChain chain)
        {
            mimeType.AcceptTypes = new MimeTypeList(MimeType.Json);
        }
    }
}