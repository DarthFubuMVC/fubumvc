using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class Using_custom_media_readers_and_writers
    {
        [Test]
        public void use_a_custom_media_reader()
        {
            using (var host = FubuRuntime.Basic())
            {
                host.Scenario(x =>
                {
                    x.Post.Input<SomeInput>();

                    x.Request
                        .AppendHeader("x-name", "Jeremy")
                        .ContentType("some/input")
                        .Accepts("some/output");

                    x.ContentShouldContain("The name is Jeremy");
                    x.StatusCodeShouldBe(HttpStatusCode.OK);
                });
            }
        }

        [Test]
        public void use_a_custom_projection_as_a_writer()
        {
            using (var host = FubuRuntime.Basic())
            {
                host.Scenario(x =>
                {
                    x.Post.Input<SomeInput>();
                    x.Request
                        .AppendHeader("x-name", "Jeremy")
                        .ContentType("some/input")
                        .Accepts("some/json");

                    x.ContentShouldContain("{\"Name\":\"Jeremy\"}");
                    x.StatusCodeShouldBe(HttpStatusCode.OK);
                });
            }
        }
    }

    public class SomeProjection : Projection<SomeResource>
    {
        public SomeProjection()
        {
            Value(x => x.Name);
        }

        public override IEnumerable<string> Mimetypes
        {
            get { yield return "some/json"; }
        }
    }

    public class SomeSpecialEndpoint
    {
        public SomeResource post_some_input(SomeInput input)
        {
            return new SomeResource {Name = input.Name};
        }
    }

    public class SomeReader : IReader<SomeInput>
    {
        public IEnumerable<string> Mimetypes
        {
            get { yield return "some/input"; }
        }

        public Task<SomeInput> Read(string mimeType, IFubuRequestContext context)
        {
            return new SomeInput {Name = context.Request.GetSingleHeader("x-name")}.ToCompletionTask();
        }
    }

    public class SomeWriter : IMediaWriter<SomeResource>
    {
        public Task Write(string mimeType, IFubuRequestContext context, SomeResource resource)
        {
            return context.Writer.Write(mimeType, "The name is " + resource.Name);
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return "some/output"; }
        }
    }

    public class SomeInput
    {
        public string Name { get; set; }
    }

    public class SomeResource
    {
        public string Name { get; set; }
    }
}