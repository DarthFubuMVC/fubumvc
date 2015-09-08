using FubuCore;
using FubuCore.Binding;
using FubuCore.Configuration;
using FubuCore.Conversion;
using FubuCore.Dates;
using FubuCore.Formatting;
using FubuCore.Logging;
using FubuCore.Reflection;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.JavascriptRouting;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Assets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Core.Json;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Aggregation;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.Runtime.SessionState;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;

namespace FubuMVC.Core.Registration
{
    /// <summary>
    /// The core runtime service registry for a FubuMVC application
    /// </summary>
    public class CoreServiceRegistry : ServiceRegistry
    {
        public CoreServiceRegistry(string mode)
        {
            SetServiceIfNone<IAsyncHandling, AsyncHandling>();

            AddService<IDeactivator, MiddlewareDeactivator>();

            SetServiceIfNone<IAggregator, Aggregator>();
            SetServiceIfNone<IRequestCompletion, RequestCompletion>();

            SetServiceIfNone<IRequestData, FubuMvcRequestData>();

            For<AppReloaded>().Use(new AppReloaded());

            SetServiceIfNone<IDiagnosticAssets, DiagnosticAssetsCache>().Singleton();

            // HAck alert. Inconsistent usage in the app, so you're stuck with this.
            var stringifier = new Stringifier();
            For<Stringifier>().Use(stringifier);
            For<IStringifier>().Use(stringifier);

            AddService(new TypeDescriptorCache());

            SetServiceIfNone<IConditionalService, ConditionalService>();
            SetServiceIfNone<IOutputWriter, OutputWriter>();

            SetServiceIfNone<IUrlRegistry, UrlRegistry>();
            SetServiceIfNone<IChainUrlResolver, ChainUrlResolver>();

            SetServiceIfNone<IFlash, FlashProvider>();
            SetServiceIfNone<IRequestDataProvider, RequestDataProvider>();

            SetServiceIfNone<IFubuRequest, FubuRequest>();
            SetServiceIfNone<IPartialFactory, PartialFactory>();
            SetServiceIfNone<IContinuationProcessor, ContinuationProcessor>();

            SetServiceIfNone<IDisplayFormatter, DisplayFormatter>();
            SetServiceIfNone<IChainResolver, ChainResolutionCache>().Singleton();

            SetServiceIfNone<IEndpointService, EndpointService>();


            SetServiceIfNone<ITypeDescriptorCache, TypeDescriptorCache>().Singleton();

            SetServiceIfNone<ISessionState, SimpleSessionState>();


            SetServiceIfNone<IFubuRequestContext, FubuRequestContext>();
            SetServiceIfNone<IFileSystem, FileSystem>();


            SetServiceIfNone<IObjectConverter, ObjectConverter>();

            SetServiceIfNone<IResourceNotFoundHandler, DefaultResourceNotFoundHandler>();


            SetServiceIfNone<ILogger, Logger>();
            AddService<ILogModifier, LogRecordModifier>();

            SetServiceIfNone<IClock, Clock>().Singleton();
            SetServiceIfNone<ITimeZoneContext, MachineTimeZoneContext>();
            SetServiceIfNone<ISystemTime, SystemTime>();

            SetServiceIfNone<IExceptionHandlingObserver, ExceptionHandlingObserver>();
            SetServiceIfNone<IAsyncCoordinator, AsyncCoordinator>();

            SetServiceIfNone<IPartialInvoker, PartialInvoker>();

            AddService<IHttpContentEncoding, GZipHttpContentEncoding>();
            AddService<IHttpContentEncoding, DeflateHttpContentEncoding>();
            SetServiceIfNone<IHttpContentEncoders, HttpContentEncoders>();

            SetServiceIfNone<ICookies, Cookies>();

            if (mode.InDevelopment())
            {
                SetServiceIfNone<IAssetTagBuilder, DevelopmentModeAssetTagBuilder>();
            }
            else
            {
                SetServiceIfNone<IAssetTagBuilder, AssetTagBuilder>();
            }

            SetServiceIfNone<IJavascriptRouteData, JavascriptRouteData>();

            SetServiceIfNone(typeof (IValues<>), typeof (SimpleValues<>));
            SetServiceIfNone(typeof (IValueSource<>), typeof (ValueSource<>));

            SetServiceIfNone<IProjectionRunner, ProjectionRunner>();
            SetServiceIfNone(typeof (IProjectionRunner<>), typeof (ProjectionRunner<>));
            SetServiceIfNone<IProjectionRunner, ProjectionRunner>();
            SetServiceIfNone(typeof (IProjectionRunner<>), typeof (ProjectionRunner<>));

            SetServiceIfNone<ISettingsProvider, SettingsProvider>();
            AddService<ISettingsSource>(new AppSettingsSettingSource(SettingCategory.environment));

            SetServiceIfNone<IAssetFinder, AssetFinderCache>().Singleton();

            SetServiceIfNone<IClientMessageCache, ClientMessageCache>().Singleton();

            SetServiceIfNone<IJsonSerializer, NewtonSoftJsonSerializer>();

            SetServiceIfNone<IPartialInvoker, PartialInvoker>();
            AddService<IActivator>(typeof(DisplayConversionRegistryActivator));
        }
    }
}