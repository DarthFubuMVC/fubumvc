using System.Collections.Generic;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class Using_custom_media_readers_and_writers
    {
        [Test]
        public void use_a_custom_media_reader()
        {
            using (var host = FubuApplication.DefaultPolicies().StructureMap().RunInMemory())
            {
                host.Scenario(x => {
                    x.Post.Input<SomeInput>();
                    x.Request
                        .Header("x-name", "Jeremy")
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
            using (var host = FubuApplication.DefaultPolicies().StructureMap().RunInMemory())
            {
                host.Scenario(x =>
                {
                    x.Post.Input<SomeInput>();
                    x.Request
                        .Header("x-name", "Jeremy")
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
            return new SomeResource{Name = input.Name};
        }
    }

    public class SomeReader : IReader<SomeInput>
    {
        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return "some/input";
            }
        }
        public SomeInput Read(string mimeType, IFubuRequestContext context)
        {
            return new SomeInput {Name = context.Request.GetSingleHeader("x-name")};
        }
    }

    public class SomeWriter : IMediaWriter<SomeResource>
    {
        public void Write(string mimeType, IFubuRequestContext context, SomeResource resource)
        {
            context.Writer.Write(mimeType, "The name is " + resource.Name);
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return "some/output";
            }
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