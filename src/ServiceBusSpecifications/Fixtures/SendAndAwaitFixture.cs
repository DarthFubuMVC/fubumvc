using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus;
using Serenity.Fixtures;
using ServiceNode;
using StoryTeller;

namespace ServiceBusSpecifications.Fixtures
{
    public class SendAndAwaitFixture : Fixture
    {
        private Task _task;

        public SendAndAwaitFixture()
        {
            Title = "Send and Await an Acknowledgement";
        }

        [FormatAs("Send a message that we expect to succeed and wait for the ack")]
        public void SendMessageSuccessfully()
        {
            _task = Retrieve<IServiceBus>().SendAndWait(new SimpleMessage());
        }

        [FormatAs("Send a message that will fail with an AmbiguousMatchException exception")]
        public void SendMessageUnsuccessfully()
        {
            _task = Retrieve<IServiceBus>().SendAndWait(new ServiceNode.ErrorMessage());
        }

        [FormatAs("The acknowledgement was received within {seconds} seconds")]
        public bool AckIsReceived(int seconds)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                _task.Wait(seconds.Seconds());
            }
            catch (Exception)
            {
            }
            sw.Stop();
            return sw.Elapsed.Seconds < seconds;
        }

        [FormatAs("The acknowledgement was successful")]
        public bool AckWasSuccessful()
        {
            StoryTellerAssert.Fail(_task.IsFaulted, () => _task.Exception.ToString());

            return true;
        }

        [FormatAs("The acknowledgment failed and contained the message {message}")]
        public bool TheAckFailedWithMessage(string message)
        {
            StoryTellerAssert.Fail(_task.Exception == null, "The task exception is null");

            StoryTellerAssert.Fail(!_task.Exception.InnerExceptions.First().ToString().Contains(message), "The actual exception text was:\n" + _task.Exception.ToString());

            return true;
        }
    }
}