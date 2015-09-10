using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Dates;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.StructureMap;
using HtmlTags;
using StoryTeller;
using StoryTeller.Conversion;
using StoryTeller.Engine;
using StoryTeller.Equivalence;
using StructureMap;

namespace Serenity
{
    /* TODO
     * 
     * 1.) Subsystems for remote work, add to FubuRegistry

     * 11.) Figure out how to attach message tracking. Use 

     * 13.) Apply MessageContextualInfoProvider from FT testing
     * 14.) Look closely at FubuTransportSystem and TestNodes
     */

    public class SerenitySystem : SerenitySystem<FubuRegistry>
    {
    }

    public class SerenitySystem<T> : ISystem where T : FubuRegistry, new()
    {
        public readonly CellHandling CellHandling = new CellHandling(new EquivalenceChecker(), new Conversions());
        public readonly T Registry = new T();
        private FubuRuntime _runtime;
        private Task _warmup;
        private StructureMapServiceFactory _factory;

        public SerenitySystem()
        {
            Registry.Services.ReplaceService<ISystemTime, SystemTime>().Singleton();
            Registry.Services.ReplaceService<IClock, Clock>().Singleton();
            Registry.Features.Diagnostics.Enable(TraceLevel.Verbose);
            Registry.Services.ForSingletonOf<SecuritySettings>();
            Registry.Services.For<IAfterNavigation>().Use<NulloAfterNavigation>();
            Registry.Services.ForSingletonOf<IBrowserLifecycle>().Use("Browser Lifecycle", () =>
            {
                var browserType = BrowserFactory.DetermineBrowserType(DefaultBrowser);
                return BrowserFactory.GetBrowserLifecyle(browserType);
            });

            injectJavascriptErrorDetection();

            Registry.Mode = "testing";
        }

        private void injectJavascriptErrorDetection()
        {
            var js =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(typeof (SerenitySystem), "errorCollector.js")
                    .ReadAllText();
            var text =
                new HtmlTag("script").Attr("type", "text/javascript")
                    .Text("\n\n" + js + "\n\n")
                    .Encoded(false)
                    .ToString();
            Registry.AlterSettings<OwinSettings>(owin =>
            {
                owin.AddMiddleware<HtmlHeadInjectionMiddleware>().Arguments.With(new InjectionOptions
                {
                    Content = c => text
                });
            });
        }

        public BrowserType? DefaultBrowser { get; set; }

        /// <summary>
        /// Register a policy about what to do after navigating the browser to handle issues
        /// like being redirected to a login screen
        /// </summary>
        public void AfterNavigation<TNavigation>() where TNavigation : IAfterNavigation
        {
            Registry.Services.ReplaceService<IAfterNavigation, TNavigation>();
        }

        public FubuRuntime Runtime
        {
            get
            {
                if (_warmup != null)
                {
                    _warmup.Wait();
                }

                if (_runtime == null)
                {
                    throw new InvalidOperationException(
                        "This property is not available until Storyteller either \"warms up\" the system or until the first specification is executed");
                }

                return _runtime;
            }
        }


        protected virtual void beforeAll()
        {
            // Nothing
        }

        protected virtual void afterEach(IContainer scope, ISpecContext context)
        {
            // nothing
        }

        protected virtual void beforeEach(IContainer scope)
        {
            // nothing
        }

        protected virtual void afterAll()
        {
            // nothing
        }

        void IDisposable.Dispose()
        {
            if (_runtime != null)
            {
                afterAll();
                _runtime.Dispose();
            }
        }

        CellHandling ISystem.Start()
        {
            return CellHandling;
        }

        IExecutionContext ISystem.CreateContext()
        {
            if (_warmup != null)
            {
                _warmup.Wait();
            }
            else if (_runtime == null)
            {
                startAll();
            }

            Runtime.Get<IClock>().As<Clock>().Live();
            _factory.Get<SecuritySettings>().Reset();
            _factory.StartNewScope();
            beforeEach(_factory.Container);

            return new SerenityContext(this);
        }

        private void startAll()
        {
            _runtime = new FubuRuntime(Registry);
            _factory = _runtime.Get<IServiceFactory>().As<StructureMapServiceFactory>();
            _factory.Get<SecuritySettings>().Reset(); // force it to be created from the parent
            beforeAll();
        }

        Task ISystem.Warmup()
        {
            _warmup = Task.Factory.StartNew(startAll);
            return _warmup;
        }

        /* TODO -- figure out what to do here
        public void StartListeningForMessages()
        {
            MessageHistory.StartListening(_remoteSubSystems.Select(x => x.Runner).ToArray());
        }
         */


        public class SerenityContext : IExecutionContext
        {
            private readonly string _sessionTag = Guid.NewGuid().ToString();
            private readonly SerenitySystem<T> _parent;

            public SerenityContext(SerenitySystem<T> parent)
            {
                _parent = parent;
            }

            void IDisposable.Dispose()
            {
            }

            public void BeforeExecution(ISpecContext context)
            {
                GetService<IChainExecutionHistory>().CurrentSessionTag = _sessionTag;

                // TODO -- figure out how to do this
                //_system.Application.Navigation.Logger = new ContextualNavigationLogger(context);
            }

            public void AfterExecution(ISpecContext context)
            {
                var reporter = new RequestReporter(_parent._runtime);
                var requestLogs =
                    GetService<IChainExecutionHistory>()
                        .RecentReports()
                        .Where(x => x.SessionTag == _sessionTag)
                        .ToArray();

                reporter.Append(requestLogs);

                context.Reporting.Log(reporter);

                _parent.afterEach(_parent._factory.Container, context);

                _parent._factory.TeardownScope();
            }

            public TService GetService<TService>()
            {
                return _parent._runtime.Get<TService>();
            }
        }
    }
}