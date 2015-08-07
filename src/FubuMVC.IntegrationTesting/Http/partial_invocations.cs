using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Http
{
    [TestFixture]
    public class partial_invocations
    {
        [Test]
        public void add_their_cookies_to_the_parent_request()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Input<NonPartialInput>();
                _.Response.Cookies().Count().ShouldBe(1);
            });
        }

        [Test]
        public void fast_invocation()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<PartialInvocationEndpoints>(x => x.get_fast_partial());
                _.ContentShouldBe("The name was Malcolm Reynolds");
            });
        }
    }

    public class PartialInvocationEndpoints
    {
        private readonly IPartialInvoker _partialInvoker;
        private readonly IOutputWriter _outputWriter;
        private readonly IFubuRequest _request;
        private readonly IChainResolver _resolver;

        public PartialInvocationEndpoints(IPartialInvoker partialInvoker, IOutputWriter outputWriter, IFubuRequest request,
            IChainResolver resolver)
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
            var output = _partialInvoker.InvokeFast(chain, input).As<FastOutput>();

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
            throw new NotImplementedException("I should not be called");
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