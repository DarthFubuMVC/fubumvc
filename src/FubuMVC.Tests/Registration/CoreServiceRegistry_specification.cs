using System.Linq;
using FubuCore;
using FubuCore.Conversion;
using FubuCore.Formatting;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.SessionState;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class CoreServiceRegistry_specification
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof (TImplementation));
        }

        [Test]
        public void async_coordinator_is_registered()
        {
            registeredTypeIs<IAsyncCoordinator, AsyncCoordinator>();
        }

        [Test]
        public void exception_observer_is_registered()
        {
            registeredTypeIs<IExceptionHandlingObserver, ExceptionHandlingObserver>();
        }

        [Test]
        public void registers_Cookies()
        {
            registeredTypeIs<ICookies, Cookies>();
        }

        [Test]
        public void IFlash_is_registered()
        {
            registeredTypeIs<IFlash, FlashProvider>();
        }

        [Test]
        public void IOutputWriter_is_registered()
        {
            registeredTypeIs<IOutputWriter, OutputWriter>();
        }

        [Test]
        public void IPartialFactory_is_registered()
        {
            registeredTypeIs<IPartialFactory, PartialFactory>();
        }

        [Test]
        public void IRequestDataProvider_is_registered()
        {
            registeredTypeIs<IRequestDataProvider, RequestDataProvider>();
        }

        [Test]
        public void a_value_of_stringifier_is_registered()
        {
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor(typeof (Stringifier)).Value.ShouldBeOfType
                <Stringifier>();
        }

        [Test]
        public void default_chain_resolver_is_registered()
        {
            ServiceRegistry.ShouldBeSingleton(typeof (ChainResolutionCache)).ShouldBeTrue();
            registeredTypeIs<IChainResolver, ChainResolutionCache>();
        }

        [Test]
        public void default_json_reader_is_JavascriptDeserializer_flavor()
        {
            registeredTypeIs<IJsonReader, JavaScriptJsonReader>();
        }

        [Test]
        public void display_formatter_is_registered()
        {
            registeredTypeIs<IDisplayFormatter, DisplayFormatter>();
        }

        [Test]
        public void endpoint_service_is_registered()
        {
            registeredTypeIs<IEndpointService, EndpointService>();
        }

        [Test]
        public void file_system_is_registered()
        {
            registeredTypeIs<IFileSystem, FileSystem>();
        }

        [Test]
        public void log_record_modifier_is_registered()
        {
            BehaviorGraph.BuildEmptyGraph().Services.ServicesFor<ILogModifier>()
                .Any(x => x.Type == typeof (LogRecordModifier)).ShouldBeTrue();
        }

        [Test]
        public void object_converter_is_registered()
        {
            registeredTypeIs<IObjectConverter, ObjectConverter>();
        }



        [Test]
        public void setter_binder_is_registered()
        {
            registeredTypeIs<ISetterBinder, SetterBinder>();
        }

        [Test]
        public void should_register_the_files_service_from_the_behavior_graph_into_the_container()
        {
            var graph = BehaviorGraph.BuildEmptyGraph();
            graph.Services.DefaultServiceFor<IFubuApplicationFiles>().Value
                .ShouldBeTheSameAs(graph.Files);
        }


        [Test]
        public void the_conditional_service_is_registered()
        {
            registeredTypeIs<IConditionalService, ConditionalService>();
        }

        [Test]
        public void url_registry_is_registered()
        {
            registeredTypeIs<IUrlRegistry, UrlRegistry>();
        }

		[Test]
		public void chain_url_resolver_is_registered()
		{
			registeredTypeIs<IChainUrlResolver, ChainUrlResolver>();
		}

        [Test]
        public void should_be_a_value_for_app_reloaded()
        {
            var services = BehaviorGraph.BuildEmptyGraph().Services;
            services.DefaultServiceFor<AppReloaded>().Value.ShouldBeOfType<AppReloaded>();
        }
    }
}