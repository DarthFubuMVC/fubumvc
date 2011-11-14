using System.Net;
using FubuMVC.Core.Continuations;
using NUnit.Framework;

namespace FubuMVC.Tests.Continuations
{
    [TestFixture]
    public class RedirectToMethodsTester
    {
        [Test]
        public void Stop()
        {
            Stop<StubRedirectable>.With(HttpStatusCode.NotModified)
                .RedirectTo.AssertWasEndedWithStatusCode(HttpStatusCode.NotModified);
        }

        [Test]
        public void Transfer_to_destination()
        {
            var destination = new Input1();

            Transfer<StubRedirectable>.To(destination)
                .RedirectTo.AssertWasTransferedTo(destination);
        }

        [Test]
        public void Transfer_to_action()
        {
            Transfer<StubRedirectable>.To<ActionTarget>(x => x.Go(null))
                .RedirectTo.AssertWasTransferedTo<ActionTarget>(x => x.Go(null));
        }

        [Test]
        public void Redirect_to_destination()
        {
            var destination = new Input1();

            Redirect<StubRedirectable>.To(destination)
                .RedirectTo.AssertWasRedirectedTo(destination);
        }

        [Test]
        public void Redirect_to_action()
        {
            Redirect<StubRedirectable>.To<ActionTarget>(x => x.Go(null))
                .RedirectTo.AssertWasRedirectedTo<ActionTarget>(x => x.Go(null));
        }


        public class ActionTarget
        {
            public void Go(Input1 input){}
        }
        public class Input1{}

        public class StubRedirectable : IRedirectable
        {
            public FubuContinuation RedirectTo
            {
                get;
                set;
            }
        }
    }
}