using Bottles;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Conversion;
using FubuCore.Formatting;
using FubuCore.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Routing;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.Core.SessionState;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core
{
    public class CoreServiceRegistry : ServiceRegistry
    {
        public CoreServiceRegistry()
        {
            SetServiceIfNone(new Stringifier());
            AddService(new TypeDescriptorCache());


            SetServiceIfNone<IOutputWriter, OutputWriter>();

            SetServiceIfNone<IUrlRegistry, UrlRegistry>();
            SetServiceIfNone<IUrlTemplatePattern, NulloUrlTemplate>();
            SetServiceIfNone<IJsonWriter, JsonWriter>();

            SetServiceIfNone<IFlash, FlashProvider>();
            SetServiceIfNone<IRequestDataProvider, RequestDataProvider>();

            SetServiceIfNone<IFubuRequest, FubuRequest>();
            SetServiceIfNone<IPartialFactory, PartialFactory>();


            SetServiceIfNone<IDisplayFormatter, DisplayFormatter>();
            SetServiceIfNone<IChainResolver, ChainResolutionCache>();

            SetServiceIfNone<IEndpointService, EndpointService>();


            SetServiceIfNone<ITypeDescriptorCache, TypeDescriptorCache>();


            SetServiceIfNone<IJsonReader, JavaScriptJsonReader>();

            SetServiceIfNone<ISessionState, SimpleSessionState>();

            SetServiceIfNone<IPartialInvoker, PartialInvoker>();


            SetServiceIfNone<IFileSystem, FileSystem>();

            SetServiceIfNone<IRoutePolicy, StandardRoutePolicy>();

            SetServiceIfNone<IObjectConverter, ObjectConverter>();

            SetServiceIfNone<ISmartRequest, FubuSmartRequest>();



            AddService<IActivator>(typeof (DisplayConversionRegistryActivator));

            AddService<IFormatter>(typeof (JsonFormatter));
            AddService<IFormatter>(typeof (XmlFormatter));

            SetServiceIfNone<IEtagCache, EtagCache>();

            SetServiceIfNone<IConditionalService, ConditionalService>();
        }
    }
}