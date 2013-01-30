using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class ServiceRegistryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Singleton]
        public class MySingletonService
        {
        }

        [Test]
        public void ClearAll()
        {
            var registry1 = new FakeRegistryThing();
            var registry2 = new FakeRegistryThing();
            var registry3 = new FakeRegistryThing();
            var registry4 = new FakeRegistryThing();

            var services = new ServiceRegistry();
            services.AddService(registry1);
            services.AddService(registry2);
            services.AddService(registry3);
            services.AddService(registry4);

            services.ClearAll<FakeRegistryThing>();

            services.ToGraph().FindAllValues<FakeRegistryThing>().Any().ShouldBeFalse();
        }

        [Test]
        public void GetAllValues()
        {
            var registry1 = new FakeRegistryThing();
            var registry2 = new FakeRegistryThing();
            var registry3 = new FakeRegistryThing();
            var registry4 = new FakeRegistryThing();

            var services = new ServiceRegistry();
            services.AddService(registry1);
            services.AddService(registry2);
            services.AddService(registry3);
            services.AddService(registry4);

            services.AddService<FakeRegistryThing, FakeRegistryThing>();

            services.ToGraph().FindAllValues<FakeRegistryThing>()
                .ShouldHaveTheSameElementsAs(registry1, registry2, registry3, registry4);
        }

        [Test]
        public void fill_service_that_has_something_already_does_not_add_values()
        {
            var registry1 = new FakeRegistryThing();
            var registry2 = new FakeRegistryThing();

            var services = new ServiceRegistry();
            services.AddService(registry1);

            services.SetServiceIfNone(registry2);

            services.ToGraph().FindAllValues<FakeRegistryThing>().ShouldHaveTheSameElementsAs(registry1);
        }

        [Test]
        public void fill_service_with_nothing_should_add_the_service()
        {
            var registry1 = new FakeRegistryThing();

            var services = new ServiceRegistry();
            services.SetServiceIfNone(registry1);

            services.ToGraph().FindAllValues<FakeRegistryThing>().ShouldHaveTheSameElementsAs(registry1);
        }


        [Test]
        public void replace_service_by_specifying_a_value()
        {
            var graph = new BehaviorGraph();
            var resolver = MockRepository.GenerateMock<IObjectResolver>();

            var services = new ServiceRegistry();

            services.ReplaceService(resolver);
            services.As<IServiceRegistration>().Apply(graph.Services);

            graph.Services.DefaultServiceFor<IObjectResolver>().Value.ShouldBeTheSameAs(resolver);
        }


        [Test]
        public void replace_service_by_specifying_types()
        {
            var graph = new BehaviorGraph();

            var services = new ServiceRegistry();


            services.ReplaceService<IOutputWriter, FakeOutputWriter>();
            services.As<IServiceRegistration>().Apply(graph.Services);


            graph.Services.DefaultServiceFor<IOutputWriter>().Type.ShouldEqual(typeof (FakeOutputWriter));
        }

        [Test]
        public void should_add_object_def_directly()
        {
            var registry1 = new FakeRegistryThing();

            var services = new ServiceRegistry();
            var objectDef = new ObjectDef(typeof (FakeRegistryThing)){
                Value = registry1
            };
            services.AddService(typeof (FakeRegistryThing), objectDef);

            services.ToGraph().DefaultServiceFor<FakeRegistryThing>().ShouldEqual(objectDef);
        }

        [Test]
        public void should_be_a_singleton_when_the_concrete_type_has_the_singleton_attribute()
        {
            ServiceRegistry.ShouldBeSingleton(typeof (MySingletonService))
                .ShouldBeTrue();
        }

        [Test]
        public void should_be_singleton_is_true_for_any_type_ending_in_Cache()
        {
            ServiceRegistry.ShouldBeSingleton(typeof (IPropertyBinderCache)).ShouldBeTrue();
            ServiceRegistry.ShouldBeSingleton(typeof (IModelBinderCache)).ShouldBeTrue();
        }
    }

    public class FakeRegistryThing{}

    public class FakeOutputWriter : IOutputWriter
    {
        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            throw new NotImplementedException();
        }

        public void Write(string contentType, string renderedOutput)
        {
            throw new NotImplementedException();
        }

        public void Write(string renderedOutput)
        {
            throw new NotImplementedException();
        }

        public void RedirectToUrl(string url)
        {
            throw new NotImplementedException();
        }

        public void AppendCookie(Cookie cookie)
        {
            throw new NotImplementedException();
        }

        public void AppendHeader(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void Write(string contentType, Action<Stream> output)
        {
            throw new NotImplementedException();
        }

        public void WriteResponseCode(HttpStatusCode status, string description)
        {
            throw new NotImplementedException();
        }

        public IRecordedOutput Record(Action action)
        {
            throw new NotImplementedException();
        }

        public void Replay(IRecordedOutput output)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }
    }

    public static class ServiceRegistryExtensions
    {
        public static ServiceGraph ToGraph(this ServiceRegistry registry)
        {
            var behaviorGraph = new BehaviorGraph();
            registry.As<IServiceRegistration>().Apply(behaviorGraph.Services);

            return behaviorGraph.Services;
        }
    }
}