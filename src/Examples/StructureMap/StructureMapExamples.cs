using System.IO;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Json;
using StructureMap;

namespace Examples.StructureMap
{
    // SAMPLE: SM-your-own-services
    public interface IMyService{}
    public class MyService : IMyService{}
    // ENDSAMPLE


    // SAMPLE: SM-inline-configuration
    public class MyFubuApp : FubuRegistry
    {
        public MyFubuApp()
        {
            // doing the registration inline
            Services.For<IMyService>().Use<MyService>();
        }
    }
    // ENDSAMPLE

    // SAMPLE: SM-can-inject-dependencies-into-endpoint
    public class MyEndpoint
    {
        private readonly IMyService _service;
        private readonly IHttpRequest _request;

        // IHttpRequest would point at the current request
        public MyEndpoint(IMyService service, IHttpRequest request)
        {
            _service = service;
            _request = request;
        }
    }
    // ENDSAMPLE

    // SAMPLE: SM-using-a-registry
    public class MyServiceRegistry : Registry
    {
        public MyServiceRegistry()
        {
            For<IMyService>().Use<MyService>();
        }
    }

    public class MyFubuApp2 : FubuRegistry
    {
        public MyFubuApp2()
        {
            // doing the registration inline
            Services.IncludeRegistry<MyServiceRegistry>();
        }
    }
    // ENDSAMPLE


    public class MyJilJsonSerializer : IJsonSerializer
    {
        public string Serialize(object target, bool includeMetadata = false)
        {
            throw new System.NotImplementedException();
        }

        public T Deserialize<T>(string input)
        {
            throw new System.NotImplementedException();
        }

        public T Deserialize<T>(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }

    // SAMPLE: SM-override-system-service
    public class MyFubuApp3 : FubuRegistry
    {
        public MyFubuApp3()
        {
            Services
                .For<IJsonSerializer>()
                .Use<MyJilJsonSerializer>();
        }
    }
    // ENDSAMPLE
}