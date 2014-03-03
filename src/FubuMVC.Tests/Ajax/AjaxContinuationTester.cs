using System.Reflection.Emit;
using FubuCore;
using FubuMVC.Core.Ajax;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.Ajax
{
    [TestFixture]
    public class AjaxContinuationTester
    {
        private AjaxContinuation theContinuation;

        [SetUp]
        public void SetUp()
        {
            theContinuation = new AjaxContinuation();
        }

        [Test]
        public void success_is_placed_into_the_dictionary()
        {
            theContinuation.Success = false;
            theContinuation.ToDictionary()["success"].As<bool>().ShouldBeFalse();

            theContinuation.Success = true;
            theContinuation.ToDictionary()["success"].As<bool>().ShouldBeTrue();
        }

        [Test]
        public void refresh_is_placed_int_the_dictionary()
        {
            theContinuation.ShouldRefresh = false;
            theContinuation.ToDictionary()["refresh"].As<bool>().ShouldBeFalse();

            theContinuation.ShouldRefresh = true;
            theContinuation.ToDictionary()["refresh"].As<bool>().ShouldBeTrue();
        }

        [Test]
        public void message_is_placed_into_the_dictionary_if_it_exists()
        {
            theContinuation.ToDictionary().ContainsKey("message").ShouldBeFalse();

            theContinuation.Message = "something";

            theContinuation.ToDictionary()["message"].ShouldEqual("something");
        }

        [Test]
        public void errors_are_only_written_to_the_dictionary_if_they_exist()
        {
            theContinuation.ToDictionary().ContainsKey("errors").ShouldBeFalse();

            theContinuation.Errors.Add(new AjaxError(){message = "bad!"});

            theContinuation.ToDictionary()["errors"].ShouldBeOfType<AjaxError[]>()
                .Single().message.ShouldEqual("bad!");
        }
        
        [Test]
        public void navigate_page_is_only_written_to_the_dictionary_if_it_exists()
        {
            theContinuation.ToDictionary().ContainsKey("navigatePage").ShouldBeFalse();

            theContinuation.NavigatePage = "/test";
            theContinuation.ToDictionary()["navigatePage"].ShouldEqual("/test");

        }

        [Test]
        public void Successful_builder_method()
        {
            var success = AjaxContinuation.Successful();
        
            success.Errors.Any().ShouldBeFalse();
            success.Success.ShouldBeTrue();
        }

        [Test]
        public void ForMessage_builder_method()
        {
            var continuation = AjaxContinuation.ForMessage("some message");

            continuation.Success.ShouldBeFalse();
            continuation.Message.ShouldEqual("some message");
        }

        [Test]
        public void ForMessage_via_StringToken()
        {
            var token = new SomeMessage();
            var continuation = AjaxContinuation.ForMessage(token);

            continuation.Success.ShouldBeFalse();
            continuation.Message.ShouldEqual(token.ToString());
        }

        [Test]
        public void has_data_should_return_false_if_the_key_is_not_present()
        {
            theContinuation.HasData("keyThatIsNotInData").ShouldBeFalse();
        }

        [Test]
        public void has_data_should_return_true_if_the_key_is_present()
        {
            theContinuation["keyThatIsInData"] = "foo";
            theContinuation.HasData("keyThatIsInData").ShouldBeTrue();
        }
    }

    public class SomeMessage
    {
        public override string ToString()
        {
            return "some message";
        }
    }
}