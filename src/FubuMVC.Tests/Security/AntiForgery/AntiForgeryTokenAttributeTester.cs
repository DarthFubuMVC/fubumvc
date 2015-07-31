using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.AntiForgery;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.AntiForgery
{
    [TestFixture]
    public class AntiForgeryTokenAttributeTester
    {


        [Test]
        public void should_put_an_anti_forgery_token_on_the_chain()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            BehaviorGraph.BuildFrom(registry).ChainFor<Controller1>(x => x.MethodWithAF(null))
                .FirstCall()
                .Previous.ShouldBeOfType<AntiForgeryNode>()
                .Salt.ShouldBe("abc");
                
        }


        public class Controller1
        {
            [AntiForgeryToken(Salt = "abc")]
            public Output1 MethodWithAF(Input1 input)
            {
                return null;
            }

            public Output1 MethodWithoutAF(Input1 input)
            {
                return null;
            }
        }

        public class Input1{}
        public class Output1{}
    }
}