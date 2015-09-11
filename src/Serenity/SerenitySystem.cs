using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Dates;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.TestSupport;
using FubuMVC.Core.Services.Messaging;
using FubuMVC.Core.Services.Remote;
using FubuMVC.Core.StructureMap;
using HtmlTags;
using Serenity.ServiceBus;
using StoryTeller;
using StoryTeller.Conversion;
using StoryTeller.Engine;
using StoryTeller.Equivalence;
using StructureMap;
using MessageHistory = FubuMVC.Core.Services.Messaging.Tracking.MessageHistory;

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

    public class SerenitySystem<T> : ISystem, ISubSystem, IRemoteSubsystems where T : FubuRegistry, new()
    {
        private readonly IList<ISubSystem> _subSystems = new List<ISubSystem>();
        public readonly CellHandling CellHandling = new CellHandling(new EquivalenceChecker(), new Conversions());
        public readonly T Registry = new T();
        private FubuRuntime _runtime;
        private Task _warmup;
        private StructureMapServiceFactory _factory;

        public SerenitySystem()
        {
            _subSystems.Add(this);

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

        private readonly Cache<string, RemoteSubSystem> _remoteSubSystems = new Cache<string, RemoteSubSystem>();
        private bool _isDisposed;


        ~SerenitySystem()
        {
            if (_isDisposed) return;

            this.SafeDispose();
        }

        public RemoteSubSystem RemoteSubSystemFor(string name)
        {
            return _remoteSubSystems[name];
        }

        public IEnumerable<RemoteSubSystem> RemoteSubSystems
        {
            get { return _remoteSubSystems; }
        }

        public void AddRemoteSubSystem(string name, Action<RemoteDomainExpression> configuration)
        {
            
            var system = new RemoteSubSystem(() => new RemoteServiceRunner(x =>
            {
                configuration(x);
            }));

            _remoteSubSystems[name] = system;

            _subSystems.Add(system);
        }

        public void AddSubSystem<T>() where T : ISubSystem, new()
        {
            AddSubSystem(new T());
        }

        public void AddSubSystem(ISubSystem subSystem)
        {
            _subSystems.Add(subSystem);
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
                EventAggregator.Stop();
                stopAll();
                _isDisposed = true;
            }
        }

        protected virtual void stopAll()
        {
            var succeeded = Task.WaitAll(_subSystems.Select(x => x.Stop()).ToArray(), TimeSpan.FromMinutes(1));

            if (!succeeded)
            {
                // TODO: Replace with a logger
                ConsoleWriter.Write(ConsoleColor.Yellow,
                    "WARNING: Failed to stop SerenitySystem and/or registered subsystems");
            }
        }


        CellHandling ISystem.Start()
        {
            return CellHandling;
        }

        Task ISubSystem.Stop()
        {
            return Task.Factory.StartNew(() =>
            {
                if (_runtime != null)
                {
                    _runtime.SafeDispose();
                    _runtime = null;
                }

            });
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

            _factory.Get<TransportCleanup>().ClearAll();
            RemoteSubSystems.Each(x => x.Runner.SendRemotely(new ClearAllTransports()));

            Runtime.Get<IClock>().As<Clock>().Live();
            _factory.Get<SecuritySettings>().Reset();
            _factory.StartNewScope();

            var messaging = _factory.Get<IMessagingSession>();
            messaging.ClearAll();
            RemoteSubSystems.Each(sys => sys.Runner.Messaging.AddListener(messaging));


            TestNodes.Reset();

            beforeEach(_factory.Container);

            return new SerenityContext(this);
        }

        private void startAll()
        {
            Task.WaitAll(_subSystems.Select(x => x.Start()).ToArray());

            
            MessageHistory.StartListening(_remoteSubSystems.Select(x => x.Runner).ToArray());

            beforeAll();
        }



        Task ISystem.Warmup()
        {
            _warmup = Task.Factory.StartNew(startAll);
            return _warmup;
        }


        Task ISubSystem.Start()
        {
            return Task.Factory.StartNew(() =>
            {
                _runtime = new FubuRuntime(Registry);
                _factory = _runtime.Get<IServiceFactory>().As<StructureMapServiceFactory>();
                _factory.Get<SecuritySettings>().Reset(); // force it to be created from the parent

                var messaging = _runtime.Get<IMessagingSession>();

                EventAggregator.Messaging.AddListener(messaging);
            });
        }



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

                var session = GetService<IMessagingSession>();
                context.Reporting.Log(new MessageContextualInfoProvider(session));

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