using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.OwinHost;
using FubuTestingSupport;
using NUnit.Framework;
using FubuMVC.StructureMap;
using StructureMap;
using FubuMVC.Katana;

namespace FubuMVC.IntegrationTesting.Ajax
{
    [TestFixture]
    public class AjaxContinuationProcessingTester
    {

        public class TestRegistry : FubuRegistry
        {
            public TestRegistry()
            {
                Actions.IncludeType<AjaxController>();
            }
        }

        [Test]
        public void send_message_that_gets_through_the_first_behavior_and_is_handled_by_the_last()
        {
            using (var server = FubuApplication.For<TestRegistry>().StructureMap(new Container()).RunEmbedded(port:PortFinder.FindPort(5500)))
            {
                var input = new CharacterInput
                {
                    CharacterClass = "Ninja",
                    Race = "Troll"
                };

                server.Endpoints.PostJson(input).ReadAsText().ShouldEqual("{\"success\":true,\"refresh\":false}");
            }
        }

        [Test]
        public void send_message_that_gets_caught_by_validation_behavior()
        {
            using (var server = FubuApplication.For<TestRegistry>().StructureMap(new Container()).RunEmbedded(port: PortFinder.FindPort(5500)))
            {
                var input = new CharacterInput
                {
                    CharacterClass = "Paladin",
                    Race = "Ogre"
                };

                var text = server.Endpoints.PostJson(input).ReadAsText();

                text.ShouldEqual("{\"success\":false,\"refresh\":false,\"errors\":[{\"category\":null,\"field\":\"Character\",\"label\":null,\"message\":\"Ogres cannot be Paladins!\"}]}");
            }


        }
    }

    public class CharacterValidator : BasicBehavior
    {
        private readonly IFubuRequest _request;

        public CharacterValidator(IFubuRequest request) : base(PartialBehavior.Executes)
        {
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            var input = _request.Get<CharacterInput>();
            if (input.Race == "Ogre" && input.CharacterClass == "Paladin")
            {
                var continuation = new AjaxContinuation();
                continuation.Errors.Add(new AjaxError{field = "Character", message = "Ogres cannot be Paladins!"});
                _request.Set(continuation);

                return DoNext.Stop;
            }

            return DoNext.Continue;
        }
    }

    public class AjaxController
    {
        [WrapWith(typeof(CharacterValidator))]
        public AjaxContinuation post_save_character(CharacterInput input)
        {
            return AjaxContinuation.Successful();
        }
    }

    public class CharacterInput
    {
        public int Level { get; set; }
        public string CharacterClass { get; set; }
        public string Race { get; set; }
    }
}