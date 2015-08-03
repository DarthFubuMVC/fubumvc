using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Json;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Json
{
    [TestFixture]
    public class NewtonsoftBindingReaderTester
    {
        private JsonTarget theResult;

        [SetUp]
        public void SetUp()
        {
            var json = "{Name:'Max', Age:8, Nested:{Order:5}, Array:[{Order:0}, {Order:1}, {Order:2}]}".Replace("'", "\"");

            JsonTargetEndpoint.LastTarget = null;

            using (var server = FubuRuntime.Basic())
            {
                server.Scenario(_ => {
                    _.Post.Input<JsonTarget>();
                    _.Request.Body.JsonInputIs(json);
                    _.Request.ContentType("text/json");

                });
            }

            theResult = JsonTargetEndpoint.LastTarget;
        }



        [Test]
        public void can_read_basic_properties()
        {
            theResult.Name.ShouldBe("Max");
            theResult.Age.ShouldBe(8);
        }

        [Test]
        public void can_read_nested_properties()
        {
            theResult.Nested.Order.ShouldBe(5);
        }

        [Test]
        public void can_read_enumerable_properties()
        {
            theResult.Array.Select(x => x.Order)
                .ShouldHaveTheSameElementsAs(0, 1, 2);
        }
    }

    public class JsonTargetEndpoint
    {
        public static JsonTarget LastTarget;

        [JsonBinding]
        public string post_json_target(JsonTarget target)
        {
            LastTarget = target;

            return "ok";
        }
    }

    public class JsonTarget
    {
        public string Name { get; set;}
        public int Age { get; set; }

        public NestedJsonTarget Nested { get; set; }
        public NestedJsonTarget[] Array { get; set; }
    }

    public class NestedJsonTarget
    {
        public int Order { get; set; }
    }


}