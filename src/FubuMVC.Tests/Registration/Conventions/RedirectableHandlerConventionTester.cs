using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class RedirectableHandlerConventionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry(x => { x.Actions.IncludeType<RedirectableHandlerController>(); }).BuildGraph();


            graph.Behaviors.SelectMany(x => x).Each(x => Debug.WriteLine(x));
        }

        #endregion

        private BehaviorGraph graph;

        public class RedirectableHandlerController
        {
            public string LastNameEntered;

            public Redirectable<Model1> GoNext()
            {
                return null;
            }

            public Redirectable<Model1> GoAfter(Model1 input)
            {
                return null;
            }


            public Model1 ZeroInOneOut()
            {
                return new Model1 {
                    Name = "ZeroInOneOut"
                };
            }

            public Model2 OneInOneOut(Model1 input)
            {
                return new Model2 {
                    Name = input.Name
                };
            }

            public void OneInZeroOut(Model1 input)
            {
                LastNameEntered = input.Name;
            }
        }

        [Test]
        public void should_not_attach_redirectable_handlers_to_actions_that_do_not_return_continuations()
        {
            graph.BehaviorFor<RedirectableHandlerController>(x => x.ZeroInOneOut()).Any(x => x is RedirectableNode<Model1>).ShouldBeFalse();
        }

        [Test]
        public void
            the_actions_that_return_continuations_should_have_a_continuation_handler_right_behind_the_action_in_the_chain()
        {
            graph.BehaviorFor<RedirectableHandlerController>(x => x.GoNext()).FirstCall().Next.ShouldBeOfType
                <RedirectableNode<Model1>>();
            graph.BehaviorFor<RedirectableHandlerController>(x => x.GoAfter(null)).FirstCall().Next.ShouldBeOfType
                <RedirectableNode<Model1>>();
        }
    }
}
