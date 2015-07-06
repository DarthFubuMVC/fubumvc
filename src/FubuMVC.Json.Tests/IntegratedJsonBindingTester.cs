using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Dates;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Endpoints;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Json.Tests
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

            using (var server = new IntegratedJsonBindingApplication(recorder, time).BuildApplication().RunEmbedded())
            {
                var url = server.Urls.UrlFor(typeof (IntegratedJsonBindingTarget));
                var response = post(url.ToAbsoluteUrl(server.BaseAddress), "{Name:'Josh',Child:{ChildName:'Joel'},DynamicData:{test:{name:'nested'}}}");

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine(response.ReadAsText());
                }
                response.StatusCode.ShouldEqual(HttpStatusCode.OK);

                recorder.Target.Name.ShouldEqual("Josh");
                recorder.Target.Child.ChildName.ShouldEqual("Joel");
                recorder.Target.Child.CurrentTime.ShouldEqual(now);

                var child = recorder.Target.DynamicData.Value<JObject>("test");
                child["name"].ToString().ShouldEqual("nested");
            }
        }

        private HttpResponse post(string url, string json)
        {
            WebRequest request = WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            TypeExtensions.As<HttpWebRequest>((object)request).Accept = "application/json";
            TypeExtensions.As<HttpWebRequest>((object)request).CookieContainer = new CookieContainer();
            Stream requestStream = request.GetRequestStream();

            byte[] local_2 = Encoding.Default.GetBytes(json);
            requestStream.Write(local_2, 0, local_2.Length);

            requestStream.Close();
            return request.ToHttpCall();
        }

        public class Recorder
        {
            public IntegratedJsonBindingTarget Target { get; private set; }
            
            public void Record(IntegratedJsonBindingTarget target)
            {
                Target = target;
            }
        }

        public class IntegratedJsonBindingApplication : IApplicationSource
        {
            private readonly Recorder _recorder;
            private readonly ISystemTime _time;

            public IntegratedJsonBindingApplication(Recorder recorder, ISystemTime time)
            {
                _recorder = recorder;
                _time = time;
            }

            public FubuApplication BuildApplication()
            {
                return FubuApplication
                    .For(new IntegrationJsonBindingRegistry(_time))
                    .StructureMap(new Container(x => x.For<Recorder>().Use(_recorder)));
            }
        }

        public class IntegrationJsonBindingRegistry : FubuRegistry
        {
            public IntegrationJsonBindingRegistry(ISystemTime time)
            {
                Actions.IncludeType<IntegratedJsonBindingEndpoint>();
                Models.BindPropertiesWith<CurrentTimePropertyBinder>();
                Import<JsonBinding>();

                Services(x => x.ReplaceService(time));
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