using Bottles;
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
using FubuMVC.Core.Assets.Templates;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Routing;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.SessionState;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core
{
    /// <summary>
    /// The core runtime service registry for a FubuMVC application
    /// </summary>
    public class CoreServiceRegistry : ServiceRegistry
    {
        public CoreServiceRegistry()
        {
            AddService<IDeactivator, MiddlewareDeactivator>();

            SetServiceIfNone<IRequestCompletion, RequestCompletion>();

            SetServiceIfNone<IRequestData, FubuMvcRequestData>();
            SetServiceIfNone(typeof(AppReloaded), ObjectDef.ForValue(new AppReloaded()));

            var stringifier = new Stringifier();
            SetServiceIfNone(stringifier);
            SetServiceIfNone<IStringifier>(stringifier); // Hack!
            AddService(new TypeDescriptorCache());

            SetServiceIfNone<IConditionalService, ConditionalService>();
            SetServiceIfNone<IOutputWriter, OutputWriter>();

            SetServiceIfNone<IUrlRegistry, UrlRegistry>();
            SetServiceIfNone<IChainUrlResolver, ChainUrlResolver>();
            SetServiceIfNone<IUrlTemplatePattern, NulloUrlTemplate>();

            SetServiceIfNone<IFlash, FlashProvider>();
            SetServiceIfNone<IRequestDataProvider, RequestDataProvider>();

            SetServiceIfNone<IFubuRequest, FubuRequest>();
            SetServiceIfNone<IPartialFactory, PartialFactory>();
            SetServiceIfNone<IContinuationProcessor, ContinuationProcessor>();

            SetServiceIfNone<IDisplayFormatter, DisplayFormatter>();
            SetServiceIfNone<IChainResolver, ChainResolutionCache>();

            SetServiceIfNone<IEndpointService, EndpointService>();


            SetServiceIfNone<ITypeDescriptorCache, TypeDescriptorCache>();

            SetServiceIfNone<ISessionState, SimpleSessionState>();



            SetServiceIfNone<IFubuRequestContext, FubuRequestContext>();
            SetServiceIfNone<IFileSystem, FileSystem>();


            SetServiceIfNone<IObjectConverter, ObjectConverter>();

            SetServiceIfNone<ISmartRequest, FubuSmartRequest>();
            SetServiceIfNone<IResourceNotFoundHandler, DefaultResourceNotFoundHandler>();


            SetServiceIfNone<ILogger, Logger>();
            AddService<ILogModifier, LogRecordModifier>();

            SetServiceIfNone<IClock, Clock>(x => x.IsSingleton = true);
            SetServiceIfNone<ITimeZoneContext, MachineTimeZoneContext>();
            SetServiceIfNone<ISystemTime, SystemTime>();

            SetServiceIfNone<IExceptionHandlingObserver, ExceptionHandlingObserver>();
            SetServiceIfNone<IAsyncCoordinator, AsyncCoordinator>();

            SetServiceIfNone<IPartialInvoker, PartialInvoker>();

            AddService<IHttpContentEncoding, GZipHttpContentEncoding>();
            AddService<IHttpContentEncoding, DeflateHttpContentEncoding>();
            SetServiceIfNone<IHttpContentEncoders, HttpContentEncoders>();

            SetServiceIfNone<ICookies, Cookies>();

            if (FubuMode.InDevelopment())
            {
                SetServiceIfNone<IAssetTagBuilder, DevelopmentModeAssetTagBuilder>();
            }
            else
            {
                SetServiceIfNone<IAssetTagBuilder, AssetTagBuilder>();
            }

            SetServiceIfNone<IJavascriptRouteData, JavascriptRouteData>();

            SetServiceIfNone(typeof(IValues<>), typeof(SimpleValues<>));
            SetServiceIfNone(typeof(IValueSource<>), typeof(ValueSource<>));

            SetServiceIfNone<IProjectionRunner, ProjectionRunner>();
            SetServiceIfNone(typeof(IProjectionRunner<>), typeof(ProjectionRunner<>));
            SetServiceIfNone<IProjectionRunner, ProjectionRunner>();
            SetServiceIfNone(typeof(IProjectionRunner<>), typeof(ProjectionRunner<>));

            SetServiceIfNone<ISettingsProvider, SettingsProvider>();
            AddService<ISettingsSource>(new AppSettingsSettingSource(SettingCategory.environment));

            ReplaceService<TemplateGraph, TemplateGraph>();

            SetServiceIfNone<IAssetFinder, AssetFinderCache>();
        }
    }
}