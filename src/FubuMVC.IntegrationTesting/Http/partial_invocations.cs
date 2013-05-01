using FubuMVC.Core;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Http
{
    [TestFixture]
    public class partial_invocations : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<PartialController>();
        }

        [Test]
        public void add_their_cookies_to_the_parent_request()
        {
            var response = endpoints.GetByInput(new NonPartialInput());
            response.Cookies.Count.ShouldEqual(1);
        }
    }

    public class PartialController
    {
        private readonly IPartialInvoker _partialInvoker;
        private readonly IOutputWriter _outputWriter;

        public PartialController(IPartialInvoker partialInvoker, IOutputWriter outputWriter)
        {
            _partialInvoker = partialInvoker;
            _outputWriter = outputWriter;
        }

        public string get_invokes_a_partial(NonPartialInput input)
        {
            return _partialInvoker.InvokeObject(new PartialInput());
        }

        public string get_is_a_partial(PartialInput input)
        {
            _outputWriter.AppendCookie(new Cookie("key", "value"));
            return "I am some partial text!";
        }
    }

    public class NonPartialInput
    {
    }

    public class PartialInput
    {
    }
}