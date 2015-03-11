using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Resources.Conneg;
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

        [Test]
        public void fast_invocation()
        {
            endpoints.Get<PartialController>(x => x.get_fast_partial())
                .ReadAsText().ShouldEqual("The name was Malcolm Reynolds");
        }
    }

    public class PartialController
    {
        private readonly IPartialInvoker _partialInvoker;
        private readonly IOutputWriter _outputWriter;
        private readonly IFubuRequest _request;
        private readonly IChainResolver _resolver;

        public PartialController(IPartialInvoker partialInvoker, IOutputWriter outputWriter, IFubuRequest request, IChainResolver resolver)
        {
            _partialInvoker = partialInvoker;
            _outputWriter = outputWriter;
            _request = request;
            _resolver = resolver;
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

        public string get_fast_partial()
        {
            var input = new FastInput {Name = "Malcolm Reynolds"};
            _request.Set(input);

            var chain = _resolver.FindUniqueByType(typeof (FastInput));
            _partialInvoker.InvokeFast(chain);

            var output = _request.Get<FastOutput>();

            return "The name was " + output.Name;
        }

        public FastOutput FastPartial(FastInput input)
        {
            return new FastOutput {Name = input.Name};
        }
    }

    public class FastInput
    {
        public string Name { get; set; }
    }

    public class FastOutput
    {
        public string Name { get; set; }
    }

    public class NonPartialInput
    {
    }

    public class PartialInput
    {
    }

    public class FastOutputWriter : IMediaWriter<FastOutput>
    {
        public void Write(string mimeType, IFubuRequestContext context, FastOutput resource)
        {
            throw new System.NotImplementedException("I should not be called");
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return MimeType.Html.Value;
                yield return MimeType.Json.Value;
            }
        }
    }
}