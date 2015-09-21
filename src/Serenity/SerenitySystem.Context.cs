using System;
using System.Linq;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Diagnostics;
using Serenity.ServiceBus;
using StoryTeller;
using StoryTeller.Engine;

namespace Serenity
{
    public partial class SerenitySystem<T>
    {
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

                if (_parent._runtime.Get<TransportSettings>().Enabled)
                {
                    var session = GetService<IMessagingSession>();
                    context.Reporting.Log(new MessageContextualInfoProvider(session));

                }


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