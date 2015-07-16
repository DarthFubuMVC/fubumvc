using System.Collections.Generic;
using FubuMVC.Core;
using FubuTransportation.Configuration;
using FubuTransportation.Diagnostics;
using FubuTransportation.TestSupport;
using Serenity;
using StoryTeller;

namespace FubuTransportation.Serenity
{
    public class FubuTransportSystem<T> : FubuMvcSystem<T> where T : IApplicationSource, new()
    {
        public FubuTransportSystem() : this(DetermineSettings())
        {
        }

        public FubuTransportSystem(string parallelDirectory = null, string physicalPath = null)
            : this(DetermineSettings(parallelDirectory, physicalPath))
        {
        }

        public FubuTransportSystem(ApplicationSettings settings) : base(settings)
        {
            FubuTransport.SetupForTesting(); // Uses FubuMode.SetUpTestingMode();

            OnStartup<IMessagingSession>(x => FubuMVC.Core.Services.Messaging.EventAggregator.Messaging.AddListener(x));

            // Clean up all the existing queue state to prevent test pollution
            OnContextCreation<TransportCleanup>(cleanup => {
                cleanup.ClearAll();

                RemoteSubSystems.Each(x => x.Runner.SendRemotely(new ClearAllTransports()));
            });

            OnContextCreation<IMessagingSession>(
                x =>
                {
                    x.ClearAll();
                    RemoteSubSystems.Each(sys => sys.Runner.Messaging.AddListener(x));
                });

            OnContextCreation(TestNodes.Reset);
        }

        public override void ApplyLogging(ISpecContext context)
        {
            context.Reporting.Log(new MessageContextualInfoProvider(Application.Services.GetInstance<IMessagingSession>()));
        }
    }
}