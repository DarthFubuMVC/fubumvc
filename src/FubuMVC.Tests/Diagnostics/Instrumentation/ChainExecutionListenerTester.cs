using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Diagnostics.Instrumentation
{
    
    public class ChainExecutionListenerTester : InteractionContext<ChainExecutionListener>
    {
        private void assertMessageWasLogged(object message)
        {
            MockFor<IChainExecutionLog>().AssertWasCalled(x => x.Log(message));
        }

        [Fact]
        public void debugging_is_enabled()
        {
            ClassUnderTest.IsDebugEnabled.ShouldBeTrue();
        }

        [Fact]
        public void info_is_enabled()
        {
            ClassUnderTest.IsInfoEnabled.ShouldBeTrue();
        }

        [Fact]
        public void debug_message_delegates()
        {
            var message = new object();

            ClassUnderTest.DebugMessage(message);

            assertMessageWasLogged(message);
        }

        [Fact]
        public void debug_string_delegates()
        {
            ClassUnderTest.Debug("some stuff");

            assertMessageWasLogged(new StringMessage("some stuff"));
        }

        [Fact]
        public void info_message_delegates()
        {
            var message = new object();

            ClassUnderTest.InfoMessage(message);

            assertMessageWasLogged(message);
        }

        [Fact]
        public void info_string_delegates()
        {
            ClassUnderTest.Info("some stuff");

            assertMessageWasLogged(new StringMessage("some stuff"));
        }


    }
}