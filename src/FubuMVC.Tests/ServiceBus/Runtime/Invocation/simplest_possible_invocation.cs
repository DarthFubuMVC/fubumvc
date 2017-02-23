using System.Linq;
using Xunit;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    
    public class simplest_possible_invocation : InvocationContext
    {
        [Fact]
        public void single_message_for_single_handler_one()
        {
            handler<OneHandler, TwoHandler, ThreeHandler, FourHandler>();

            var theMessage = new OneMessage();
            sendMessage(theMessage);

            TestMessageRecorder.AllProcessed.Single()
                               .ShouldMatch<OneHandler>(theMessage);
        }

        [Fact]
        public void single_message_for_single_handler_two()
        {
            handler<OneHandler, TwoHandler, ThreeHandler, FourHandler>();

            var theMessage = new TwoMessage();
            sendMessage(theMessage);

            TestMessageRecorder.AllProcessed.Single()
                               .ShouldMatch<TwoHandler>(theMessage);


        }

        [Fact]
        public void single_message_for_single_handler_three()
        {
            handler<OneHandler, TwoHandler, ThreeHandler, FourHandler>();

            var theMessage = new ThreeMessage();
            sendMessage(theMessage);

            TestMessageRecorder.AllProcessed.Single()
                               .ShouldMatch<ThreeHandler>(theMessage);
        }

        [Fact]
        public void single_message_for_single_handler_four()
        {
            handler<OneHandler, TwoHandler, ThreeHandler, FourHandler>();

            var theMessage = new FourMessage();
            sendMessage(theMessage);

            TestMessageRecorder.AllProcessed.Single()
                               .ShouldMatch<FourHandler>(theMessage);
        }
    }
}