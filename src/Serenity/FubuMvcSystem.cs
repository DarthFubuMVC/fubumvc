using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Services.Messaging.Tracking;
using FubuMVC.Core.Services.Remote;
using Serenity.Fixtures.Handlers;
using StoryTeller;
using StoryTeller.Conversion;
using StoryTeller.Engine;
using StoryTeller.Equivalence;
using StructureMap;
using StructureMap.Pipeline;

namespace Serenity
{
    public interface IRemoteSubsystems
    {
        RemoteSubSystem RemoteSubSystemFor(string name);
        IEnumerable<RemoteSubSystem> RemoteSubSystems { get; }
    }


    public class FubuMvcSystem : ISystem, ISubSystem, IRemoteSubsystems
    {
        private readonly Func<FubuRuntime> _runtimeSource;
        private bool? _externalHosting;

        private readonly IList<Action<IApplicationUnderTest>> _applicationAlterations =
            new List<Action<IApplicationUnderTest>>();

        private readonly IList<ISubSystem> _subSystems = new List<ISubSystem>();

        public readonly CellHandling CellHandling = new CellHandling(new EquivalenceChecker(), new Conversions());

        public FubuMvcSystem(Func<FubuRuntime> runtimeSource)
        {
            _runtimeSource = runtimeSource;

            _subSystems.Add(this);

            OnContextCreation(StartListeningForMessages);
        }

        public BrowserType? DefaultBrowser { get; set; }

        private readonly Cache<string, RemoteSubSystem> _remoteSubSystems = new Cache<string, RemoteSubSystem>();
        protected IApplicationUnderTest _application;

        /// <summary>
        /// *IF* your underlying container is StructureMap, this is a convenience method
        /// to modify the underlying container on Serenity system start up time
        /// </summary>
        /// <param name="configuration"></param>
        public void ModifyContainer(Action<ConfigurationExpression> configuration)
        {
            _applicationAlterations.Add(app =>
            {
                var container = app.Services.GetInstance<IContainer>();
                container.Configure(configuration);
            });
        }

        public RemoteSubSystem RemoteSubSystemFor(string name)
        {
            return _remoteSubSystems[name];
        }

        public void AddRemoteSubSystem(string name, Action<RemoteDomainExpression> configuration)
        {
            var system = new RemoteSubSystem(() => new RemoteServiceRunner(x =>
            {
                throw new Exception("REDO");
                //x.Properties[FubuMode.Testing] = true.ToString();
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

        public void StartListeningForMessages()
        {
            MessageHistory.StartListening(_remoteSubSystems.Select(x => x.Runner).ToArray());
        }

        public IEnumerable<ISubSystem> SubSystems
        {
            get { return _subSystems; }
        }

        public IApplicationUnderTest Application
        {
            get { return _application; }
        }

        /// <summary>
        /// Catch all method to call any service from the running application when
        /// the application restarts
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="startup"></param>
        public void OnStartup<TService>(Action<TService> startup)
        {
            _applicationAlterations.Add(app => startup(app.Services.GetInstance<TService>()));
        }

        /// <summary>
        /// Catch all method to perform any action from the running application when 
        /// the application restarts
        /// </summary>
        /// <param name="action"></param>
        public void OnStartup(Action action)
        {
            _applicationAlterations.Add(app => action());
        }

        public IEnumerable<RemoteSubSystem> RemoteSubSystems
        {
            get { return _subSystems.OfType<RemoteSubSystem>(); }
        }

        /// <summary>
        /// Register a policy about what to do after navigating the browser to handle issues
        /// like being redirected to a login screen
        /// </summary>
        public IAfterNavigation AfterNavigation
        {
            set { _applicationAlterations.Add(aut => aut.Navigation.AfterNavigation = value); }
        }


        /// <summary>
        /// Add an element handler to the ElementHandlers collection for driving
        /// IWebElement's with WebDriver
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddElementHandler<T>() where T : IElementHandler, new()
        {
            AddElementHandler(new T());
        }

        /// <summary>
        /// Add an element handler to the ElementHandlers collection for driving
        /// IWebElement's with WebDriver
        /// </summary>
        public void AddElementHandler(IElementHandler handler)
        {
            ElementHandlers.Handlers.Add(handler);
        }

        public BrowserType ChooseBrowserType()
        {
            if (Project.CurrentProfile.IsNotEmpty())
            {
                var profileType = BrowserType.IE;
                if (Enum.TryParse(Project.CurrentProfile, true, out profileType))
                {
                    return profileType;
                }
            }

            if (DefaultBrowser != null) return DefaultBrowser.Value;

            return WebDriverSettings.Current.Browser;
        }

        public void Dispose()
        {
            stopAll();
            _isDisposed = true;
        }

        protected virtual void stopAll()
        {
            var succeeded = Task.WaitAll(_subSystems.Select(x => x.Stop()).ToArray(), TimeSpan.FromMinutes(1));

            if (!succeeded)
            {
                // TODO: Replace with a logger
                ConsoleWriter.Write(ConsoleColor.Yellow,
                    "WARNING: Failed to stop FubuMvcSystem and/or registered subsystems");
            }
        }

        public Task Warmup()
        {
            return Task.Factory.StartNew(startAll);
        }

        public virtual IExecutionContext CreateContext()
        {
            if (_application == null)
            {
                startAll();
            }

            var context = new FubuMvcContext(this);

            _contextCreationActions.Each(x => x());

            return context;
        }

        public CellHandling Start()
        {
            return CellHandling;
        }

        protected virtual void startAll()
        {
            throw new Exception("REDO");
            //FubuMode.SetUpForDevelopmentMode();
            Task.WaitAll(_subSystems.Select(x => x.Start()).ToArray());
        }

        ~FubuMvcSystem()
        {
            if (_isDisposed) return;

            this.SafeDispose();
        }

        public void Recycle()
        {
            stopAll();
            startAll();
        }

        Task ISubSystem.Start()
        {
            return Task.Factory.StartNew(() =>
            {
                _runtime = _runtimeSource();

                var browserLifecycle = WebDriverSettings.GetBrowserLifecyle(ChooseBrowserType());
                SetupApplicationHost();

                _applicationAlterations.ToArray().Each(x => x(_application));

                throw new Exception("REDO");
                /*
                _runtime.Container.Configure(_ =>
                {
                    _.For<IApplicationUnderTest>().Use(_application);
                    _.For<IRemoteSubsystems>().Use(this);
                });
                 */

            });
        }

        private void SetupApplicationHost()
        {
            throw new NotImplementedException("NWO");
            /*
            if (_externalHosting == null)
            {
                _externalHosting = !_settings.RootUrl.IsEmpty();
            }

            _hosting = _externalHosting.Value ? (ISerenityHosting) new ExternalHosting() : new KatanaHosting();
             * */
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
                if (_application != null)
                {
                    _application.Teardown();
                    _application = null;
                }
                
            });
        }


        private readonly IList<Action> _contextCreationActions = new List<Action>();
        private FubuRuntime _runtime;
        private bool _isDisposed;

        /// <summary>
        /// Perform an action immediately after a new execution context
        /// is created immediately before the test itself is executed
        /// </summary>
        /// <param name="action"></param>
        public void OnContextCreation(Action action)
        {
            _contextCreationActions.Add(action);
        }

        /// <summary>
        /// Perform an action using a service resolved from the application 
        /// immediately after a new execution context
        /// is created immediately before the test itself is executed
        /// </summary>
        /// <param name="action"></param>
        public void OnContextCreation<T>(Action<T> action)
        {
            _contextCreationActions.Add(() => action(Application.Services.GetInstance<T>()));
        }

        // TODO -- remove this when both FT and FubuMVC are merged together
        public virtual void ApplyLogging(ISpecContext context)
        {
            // nothing
        }
    }

    public class FubuMvcSystem<T> : FubuMvcSystem where T : FubuRegistry, new()
    {
        public FubuMvcSystem(Func<FubuRuntime> runtimeSource) : base(runtimeSource)
        {
        }
    }
}