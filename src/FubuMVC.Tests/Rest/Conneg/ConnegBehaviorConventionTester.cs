using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Rest.Conneg;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Rest.Conneg
{
    [TestFixture]
    public class ConnegBehaviorConventionTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x =>
            {
                x.Actions.IncludeType<Controller1>();
                x.Actions.IncludeType<Controller2>();

                x.Media.ApplyContentNegotiationToActions(call => call.OutputType() == typeof (ViewModel3));
            });

            theGraph = registry.BuildGraph();
        }

        [Test]
        public void TESTNAME()
        {
            Assert.Fail("NWO");
        }

        //[Test]
        //public void media_node_should_not_be_applied_to_chains_not_meeting_the_criteria()
        //{
        //    theGraph.BehaviorFor<Controller1>(x => x.A()).Any(x => x is ConnegNode).ShouldBeFalse();
        //    theGraph.BehaviorFor<Controller1>(x => x.B()).Any(x => x is ConnegNode).ShouldBeFalse();

        //    theGraph.BehaviorFor<Controller2>(x => x.A()).Any(x => x is ConnegNode).ShouldBeFalse();
        //    theGraph.BehaviorFor<Controller2>(x => x.B()).Any(x => x is ConnegNode).ShouldBeFalse();
        //}

        //[Test]
        //public void the_media_node_should_be_applied_just_in_front_of_the_first_action_call()
        //{
        //    theGraph.BehaviorFor<Controller1>(x => x.D()).FirstCall().Previous.ShouldBeOfType<ConnegNode>();
        //}

        //[Test]
        //public void media_node_should_be_applied_to_chains_that_meet_the_criteria()
        //{
        //    theGraph.BehaviorFor<Controller1>(x => x.C()).Any(x => x is ConnegNode);
        //    theGraph.BehaviorFor<Controller1>(x => x.D()).Any(x => x is ConnegNode);
        //    theGraph.BehaviorFor<Controller2>(x => x.D()).Any(x => x is ConnegNode);
        //}


        public class ViewModel1
        {
        }

        public class ViewModel2
        {
        }

        public class ViewModel3
        {
        }

        public class ViewModel4
        {
        }

        public class ViewModel5
        {
        }

        public class Controller2
        {
            public ViewModel1 A()
            {
                return null;
            }

            public ViewModel2 B()
            {
                return null;
            }

            public ViewModel3 C()
            {
                return null;
            }

            public ViewModel3 D()
            {
                return null;
            }

            public ViewModel3 E()
            {
                return null;
            }
        }


        public class Controller1
        {
            public ViewModel1 A()
            {
                return null;
            }

            public ViewModel2 B()
            {
                return null;
            }

            public ViewModel3 C()
            {
                return null;
            }

            public ViewModel3 D()
            {
                return null;
            }

            public ViewModel3 E()
            {
                return null;
            }
        }
    }
}