using System.Net;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Resources.Conneg
{
    [TestFixture]
    public class when_there_are_outputs_that_would_write_headers : InteractionContext<OutputBehavior<Address>>
    {
        protected override void beforeEach()
        {
            Services.Container.Configure(x => x.For<IFubuRequestContext>().Use<FubuRequestContext>());

            var headers1 = new HttpHeaderValues();
            headers1["a"] = "1";
            headers1["b"] = "2";

            var headers2 = new HttpHeaderValues();
            headers2["c"] = "3";
            headers2["d"] = "4";

            MockFor<IFubuRequest>().Stub(x => x.Find<IHaveHeaders>()).Return(new IHaveHeaders[] {headers1, headers2});

            ClassUnderTest.WriteHeaders();
        }


        [Test]
        public void should_write_all_possible_headers()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.AppendHeader("a", "1"));
            MockFor<IOutputWriter>().AssertWasCalled(x => x.AppendHeader("b", "2"));
            MockFor<IOutputWriter>().AssertWasCalled(x => x.AppendHeader("c", "3"));
            MockFor<IOutputWriter>().AssertWasCalled(x => x.AppendHeader("d", "4"));
        }
    }


    [TestFixture]
    public class when_the_resource_cannot_be_found : InteractionContext<OutputBehavior<OutputTarget>>
    {
        protected override void beforeEach()
        {
            Services.Container.Configure(x => x.For<IFubuRequestContext>().Use<FubuRequestContext>());
            MockFor<IFubuRequest>().Stub(x => x.Get<OutputTarget>()).Return(null);


            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_write_the_404_just_in_case()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.NotFound));
        }

        [Test]
        public void should_call_through_to_the_resource_not_found_handler()
        {
            MockFor<IResourceNotFoundHandler>().AssertWasCalled(x => x.HandleResourceNotFound<OutputTarget>());
        }
    }


    public class OutputTarget
    {
    }
}