using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core.Services.Messaging;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Services.Messaging
{
    [TestFixture]
    public class MessageWaitConditionTester
    {
        [Test]
        public void watches_for_the_right_message()
        {
            var m1 = new Message();
            var m2 = new Message();
            var m3 = new Message();
            var m4 = new Message();

            var condition = new MessageWaitCondition<Message>(x => x == m4);

            condition.Receive(m1);
            condition.Received.ShouldBeNull();

            condition.Receive(m2);
            condition.Received.ShouldBeNull();

            condition.Receive(m3);
            condition.Received.ShouldBeNull();
        
            condition.Receive(m4);
            condition.Received.ShouldBeTheSameAs(m4);
        }

        [Test]
        public void wait_lasts_until_we_get_it()
        {
            var m1 = new Message();
            var m2 = new Message();
            var m3 = new Message();
            var m4 = new Message();

            var condition = new MessageWaitCondition<Message>(x => x == m4);

            Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                condition.Receive(m1);
                Thread.Sleep(100);
                condition.Receive(m2);
                Thread.Sleep(100);
                condition.Receive(m3);
                Thread.Sleep(100);
                condition.Receive(m4);

            });

            condition.Wait().ShouldBeTheSameAs(m4);

        }

        public class Message
        {
            
        }
    }
}