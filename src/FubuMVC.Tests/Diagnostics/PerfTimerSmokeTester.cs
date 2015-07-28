using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class PerfTimerSmokeTester
    {
        [Test]
        public void try_it_out()
        {
            var timer = new PerfTimer();
            timer.Start("Watch me go!");

            Thread.Sleep(50);
            timer.Mark("Here, here!");

            var t1 = Task.Factory.StartNew(() => {
                timer.Record("#1", () => {
                    Thread.Sleep(25);
                });
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                timer.Record("#2", () =>
                {
                    Thread.Sleep(80);
                });
            });

            var t3 = Task.Factory.StartNew(() =>
            {
                timer.Record("#3", () =>
                {
                    Thread.Sleep(40);
                });
            });

            var t4 = Task.Factory.StartNew(() =>
            {
                timer.Record("#4", () =>
                {
                    Thread.Sleep(5);
                });
            });

            Task.WaitAll(t1, t2, t3, t4);

            timer.Stop();

            timer.DisplayTimings(x => x.Finished);
        }


    }



    public class FakeActivator : IActivator
    {
        private readonly string _description;
        private readonly int _time;

        public FakeActivator(string description, int time)
        {
            _description = description;
            _time = time;
        }

        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            Thread.Sleep(_time);
        }

        public override string ToString()
        {
            return "Activator: "+_description;
        }
    }
}