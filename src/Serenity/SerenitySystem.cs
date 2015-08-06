using System;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Runtime;
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
     * 2.) New IBrowserLifecycle that does everything about browsers. Browser different becomes a strategy pattern
     * 3.) Register NavigationDriver, IBrowserLifecycle
     * 4.) Do the child container "scope" trick
     * 5.) Actually build the FubuRuntime
     * 6.) Allow users to specify to eagerly or lazily build the FubuRuntime
     * 7.) Allow users to specify the default browser
     * 8.) Allow users to specify whether the browser is built lazily or eagerly
     * 9.) BrowserLifecycle can give you different browsers upon request for inline
     *     cross-browser testing
     * DONE: 10. Kill IApplicationUnderTest
     * 11.) Figure out how to attach message tracking. Use 
     * 12.) Allow users to supply an AfterNavigation
     * 13.) Apply MessageContextualInfoProvider from FT testing
     * 14.) Look closely at FubuTransportSystem and TestNodes
     */

    public class SerenitySystem<T> : ISystem where T : FubuRegistry, new()
    {
        public readonly CellHandling CellHandling = new CellHandling(new EquivalenceChecker(), new Conversions());
        public readonly T Registry = new T();

        public FubuRuntime Runtime
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Register a policy about what to do after navigating the browser to handle issues
        /// like being redirected to a login screen
        /// </summary>
        public IAfterNavigation AfterNavigation
        {
            set
            {
                // TODO -- probably just register it in the container
                throw new Exception("Capture it and figure out how to use it");
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
            afterAll();
            Runtime.Dispose();
        }

        CellHandling ISystem.Start()
        {
            return CellHandling;
        }

        IExecutionContext ISystem.CreateContext()
        {
            // NEED to push the scope
            throw new NotImplementedException();
        }

        Task ISystem.Warmup()
        {
            throw new NotImplementedException();
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
                GetService<IRequestHistoryCache>().CurrentSessionTag = _sessionTag;

                // TODO -- figure out how to do this
                //_system.Application.Navigation.Logger = new ContextualNavigationLogger(context);
            }

            public void AfterExecution(ISpecContext context)
            {
                var reporter = new RequestReporter(_parent.Runtime);
                var requestLogs =
                    GetService<IRequestHistoryCache>().RecentReports().Where(x => x.SessionTag == _sessionTag).ToArray();
                reporter.Append(requestLogs);

                context.Reporting.Log(reporter);

                // TODO --- needs to be the current scope
                _parent.afterEach(null, context);
            }

            public TService GetService<TService>()
            {
                return _parent.Runtime.Get<TService>();
            }
        }
    }
}