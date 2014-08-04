using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Projections
{
    [TestFixture]
    public class end_to_end_json_serialization
    {
        private InMemoryHost _host;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _host = FubuApplication
                .For<JsonSerializationFubuRegistry>()
                .StructureMap()
                .RunInMemory();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            _host.SafeDispose();
        }

        [Test]
        public void getting_large_json_data()
        {
            _host.Scenario(x =>
            {
                x.Get.Action<JsonSerializationEndpoint>(y => y.get_large());
                x.Request.Accepts("application/json");

                x.ContentShouldContain("{\"guids\":[");
                x.ContentTypeShouldBe(MimeType.Json);
                x.StatusCodeShouldBeOk();
            });
        }

        [Test]
        public void getting_small_json_data()
        {
            _host.Scenario(x =>
            {
                x.Get.Action<JsonSerializationEndpoint>(y => y.get_small());
                x.Request.Accepts("application/json");

                x.ContentShouldContain("{\"guids\":[");
                x.ContentTypeShouldBe(MimeType.Json);
                x.StatusCodeShouldBeOk();
            });
        }

        [Test]
        public void posting_large_json_data()
        {
            _host.Scenario(x =>
            {
                var guids = new List<Guid>();
                for (var i = 0; i < 60000; i++)
                {
                    guids.Add(Guid.NewGuid());
                }
                var input = new LargeInput
                {
                    Guids = guids,
                    ExpectedCount = 60000
                };
                x.JsonData(input);

                x.ContentShouldBe("{\"successful\":true}");
                x.ContentTypeShouldBe(MimeType.Json);
                x.StatusCodeShouldBeOk();
            });
        }

        [Test]
        public void posting_small_json_data()
        {
            _host.Scenario(x =>
            {
                var guids = new List<Guid>();
                for (var i = 0; i < 10; i++)
                {
                    guids.Add(Guid.NewGuid());
                }
                var input = new SmallInput
                {
                    ExpectedCount = 10,
                    Guids = guids
                };
                x.JsonData(input);

                x.ContentShouldBe("{\"successful\":true}");
                x.ContentTypeShouldBe(MimeType.Json);
                x.StatusCodeShouldBeOk();
            });
        }
    }

    public class JsonSerializationFubuRegistry : FubuRegistry
    {
        public JsonSerializationFubuRegistry()
        {
            Actions.IncludeType<JsonSerializationEndpoint>();
        }
    }

    public interface IJsonTestData
    {
        List<Guid> Guids { get; set; }
    }

    public class JsonTestData : IJsonTestData
    {
        public List<Guid> Guids { get; set; }
    }

    public class SmallInput : IJsonTestData
    {
        public List<Guid> Guids { get; set; }
        public int ExpectedCount { get; set; }
    }

    public class LargeInput : IJsonTestData
    {
        public List<Guid> Guids { get; set; }
        public int ExpectedCount { get; set; }
    }

    public class JsonSuccess
    {
        public bool Successful { get; set; }
    }

    public class JsonSerializationEndpoint
    {
        public JsonTestData get_small()
        {
            var guids = new List<Guid>();
            for (var i = 0; i < 10; i++)
            {
                guids.Add(Guid.NewGuid());
            }
            return new JsonTestData
            {
                Guids = guids
            };
        }

        public JsonTestData get_large()
        {
            var guids = new List<Guid>();
            for (var i = 0; i < 60000; i++)
            {
                guids.Add(Guid.NewGuid());
            }
            return new JsonTestData
            {
                Guids = guids
            };
        }

        public JsonSuccess post_small(SmallInput input)
        {
            return new JsonSuccess
            {
                Successful = input.Guids.Count.Equals(input.ExpectedCount)
            };
        }

        public JsonSuccess post_large(LargeInput input)
        {
            return new JsonSuccess
            {
                Successful = input.Guids.Count.Equals(input.ExpectedCount)
            };
        }
    }

    public class JsonTestDataProjection : Projection<JsonTestData>
    {
        public JsonTestDataProjection()
        {
            Value(x => x.Guids).Name("guids");
        }
    }

    public class JsonSuccessProjection : Projection<JsonSuccess>
    {
        public JsonSuccessProjection()
        {
            Value(x => x.Successful).Name("successful");
        }
    }
}