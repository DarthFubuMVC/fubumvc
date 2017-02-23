using Xunit;

namespace FubuMVC.IntegrationTesting.Owin
{
    
    public class writing_string_output
    {
        [Fact]
        public void can_write_strings_to_the_output()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<StringEndpoint>(x => x.get_hello());
                _.ContentTypeShouldBe("text/plain");
                _.ContentShouldBe("Hello.");
            });
        }
    }

    public class StringEndpoint
    {
        public string get_hello()
        {
            return "Hello.";
        }
    }
}