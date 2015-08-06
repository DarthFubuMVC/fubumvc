using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Security.Authentication
{
    [TestFixture]
    public class unauthenticated_request_against_an_unauthenticated_route : AuthenticationHarness
    {
        [Test]
        public void nothing_happens()
        {
            var model = new PublicModel {Message = "Test"};

            Scenario(_ =>
            {
                _.Get.Input(model);
                _.StatusCodeShouldBeOk();
                _.ContentShouldBe(model.Message);
            });
        }
    }
}