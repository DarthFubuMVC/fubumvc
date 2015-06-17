using System.Threading;
using FubuTransportation.Polling;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuTransportation.Testing.Polling
{
    [TestFixture]
    public class DefaultTimerTester
    {
        [Test]
        public void start_and_callback()
        {
            var reset = new ManualResetEvent(false);

            var timer = new DefaultTimer();

            int i = 0;

            timer.Start(() => {
                i++;
                reset.Set();
                timer.Stop();
            }, 500);

            reset.WaitOne(1000);

            i.ShouldEqual(1);

            // Should only fire once because timer.Stop was called
            reset.Reset();
            reset.WaitOne(1000);

            i.ShouldEqual(1);

            timer.Dispose();
        }

        [Test]
        public void polls()
        {
            var reset = new ManualResetEvent(false);

            var timer = new DefaultTimer();

            int i = 0;

            timer.Start(() =>
            {
                i++;
                if (i == 5)
                {
                    reset.Set();
                    timer.Stop();
                }
            }, 100);

            reset.WaitOne();

            i.ShouldEqual(5);

            timer.Dispose();
        }
    }
}