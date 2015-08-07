using System;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Dates;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Json
{
    [TestFixture]
    public class IntegratedJsonBindingTester
    {
        [Test]
        public void deserializes_the_json_and_uses_the_property_binders()
        {
            var recorder = new Recorder();

            var now = DateTime.Today;
            var time = MockRepository.GenerateStub<ISystemTime>();
            time.Stub(x => x.UtcNow()).Return(now);

            using (var server = new IntegrationJsonBindingRegistry(recorder, time).ToRuntime())
            {
                server.Scenario(_ =>
                {
                    _.Post.Input<IntegratedJsonBindingTarget>().ContentType("application/json");
                    _.Request.Body.JsonInputIs(
                        "{Name:'Josh',Child:{ChildName:'Joel'},DynamicData:{test:{name:'nested'}}}");

                    _.StatusCodeShouldBeOk();
                });


                Recorder.Target.Name.ShouldBe("Josh");
                Recorder.Target.Child.ChildName.ShouldBe("Joel");
                Recorder.Target.Child.CurrentTime.ShouldBe(now);

                var child = Recorder.Target.DynamicData.Value<JObject>("test");
                child["name"].ToString().ShouldBe("nested");
            }
        }


        public class Recorder
        {
            public static IntegratedJsonBindingTarget Target { get; private set; }

            public void Record(IntegratedJsonBindingTarget target)
            {
                Target = target;
            }
        }


        public class IntegrationJsonBindingRegistry : FubuRegistry
        {
            public IntegrationJsonBindingRegistry(Recorder recorder, ISystemTime time)
            {
                Services.For<Recorder>().Use(recorder);

                Actions.IncludeType<IntegratedJsonBindingEndpoint>();
                Models.BindPropertiesWith<CurrentTimePropertyBinder>();

                Services.ReplaceService(time);
            }
        }

        public class IntegratedJsonBindingEndpoint
        {
            private readonly Recorder _recorder;

            public IntegratedJsonBindingEndpoint(Recorder recorder)
            {
                _recorder = recorder;
            }

            [JsonBinding]
            public AjaxContinuation post_integration(IntegratedJsonBindingTarget target)
            {
                _recorder.Record(target);
                return AjaxContinuation.Successful();
            }
        }


        public class IntegratedJsonBindingTarget
        {
            public string Name { get; set; }
            public IntegratedChild Child { get; set; }
            public JObject DynamicData { get; set; }
        }

        public class IntegratedChild
        {
            public string ChildName { get; set; }
            public DateTime CurrentTime { get; set; }
        }

        public class CurrentTimePropertyBinder : IPropertyBinder
        {
            public bool Matches(PropertyInfo property)
            {
                return property.Name == "CurrentTime";
            }

            public void Bind(PropertyInfo property, IBindingContext context)
            {
                var now = context.Service<ISystemTime>().UtcNow();
                property.SetValue(context.Object, now, null);
            }
        }
    }
}