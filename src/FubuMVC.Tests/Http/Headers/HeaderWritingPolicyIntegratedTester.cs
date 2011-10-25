using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Http.Headers
{
    [TestFixture]
    public class HeaderWritingPolicyIntegratedTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            theGraph = registry.BuildGraph();
        }

        [Test]
        public void should_attach_the_header_writing_behavior_for_outputs_that_implement_IHaveHeaders()
        {
            theGraph.BehaviorFor<Controller1>(x => x.M1()).IsWrappedBy(typeof (WriteHeadersBehavior))
                .ShouldBeTrue();
            theGraph.BehaviorFor<Controller1>(x => x.M2()).IsWrappedBy(typeof (WriteHeadersBehavior))
                .ShouldBeTrue();
        }

        [Test]
        public void should_not_attach_the_header_writing_behavior_for_outputs_that_do_not_implement_IHaveHeaders()
        {
            theGraph.BehaviorFor<Controller1>(x => x.M3(null)).IsWrappedBy(typeof (WriteHeadersBehavior))
                .ShouldBeFalse();

        }

        [Test]
        public void the_write_headers_behavior_is_after_the_action_call()
        {
            var chain = theGraph.BehaviorFor<Controller1>(x => x.M1());
            chain.FirstCall().Next.ShouldBeOfType<Wrapper>().BehaviorType
                .ShouldEqual(typeof (WriteHeadersBehavior));
        }


        public class Controller1
        {
            public HttpHeaderValues M1()
            {
                return new HttpHeaderValues();
            }

            public OutputWithHeaders M2()
            {
                return new OutputWithHeaders();
            }

            public void M3(OutputWithHeaders headers){}
            public NormalOutput M4()
            {
                return null;
            }
        }

        public class NormalOutput{}

        public class OutputWithHeaders : IHaveHeaders
        {
            public IEnumerable<Header> Headers
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}