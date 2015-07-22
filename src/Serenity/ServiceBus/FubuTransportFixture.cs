using System.Linq;
using FubuMVC.Core.Services.Messaging.Tracking;
using StoryTeller;

namespace Serenity.ServiceBus
{
    public abstract class FubuTransportActFixture : Fixture
    {
        public int TimeoutInMilliseconds = 10000;

        public sealed override void SetUp()
        {
            startListening();
            setup();
        }

        protected static void startListening()
        {
            MessageHistory.ClearHistory();
        }

        protected virtual void setup()
        {
            
        }

        public sealed override void TearDown()
        {
            teardown();
            waitForTheMessageProcessingToFinish();
        }

        protected void waitForTheMessageProcessingToFinish()
        {
            Wait.Until(() => !MessageHistory.Outstanding().Any() && MessageHistory.All().Any(),
                timeoutInMilliseconds: TimeoutInMilliseconds);
        }

        protected virtual void teardown()
        {
            
        }
    }
}