using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class ContinuationHandlerConventionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<ContinuationHandlerController>();
            });

        }

        #endregion

        private BehaviorGraph graph;


        public class ContinuationHandlerController
        {
            public string LastNameEntered;

            public FubuContinuation GoNext()
            {
                return null;
            }

            public FubuContinuation GoAfter(Model1 input)
            {
                return null;
            }


            public Model1 ZeroInOneOut()
            {
                return new Model1
                {
                    Name = "ZeroInOneOut"
                };
            }

            public Model2 OneInOneOut(Model1 input)
            {
                return new Model2
                {
                    Name = input.Name
                };
            }

            public void OneInZeroOut(Model1 input)
            {
                LastNameEntered = input.Name;
            }

            public Redirectable RedirectedMethod()
            {
                return new Redirectable();
            }

            public class Redirectable : IRedirectable
            {
                public FubuContinuation RedirectTo { get; set; }
            }
        }

        [Test]
        public void should_attach_continuation_handlers_to_actions_that_return_an_IRedirectable()
        {
            graph.BehaviorFor<ContinuationHandlerController>(x => x.RedirectedMethod())
                .Any(x => x is ContinuationNode)
                .ShouldBeTrue();
        }

        [Test]
        public void should_not_attach_continuation_handlers_to_actions_that_do_not_return_continuations()
        {
            graph.BehaviorFor<ContinuationHandlerController>(x => x.ZeroInOneOut()).Top.Any(x => x is ContinuationNode).
                ShouldBeFalse();
        }

        [Test]
        public void
            the_actions_that_return_continuations_should_have_a_continuation_handler_right_behind_the_action_in_the_chain
            ()
        {
            graph.BehaviorFor<ContinuationHandlerController>(x => x.GoNext()).FirstCall().Next.ShouldBeOfType
                <ContinuationNode>();
            graph.BehaviorFor<ContinuationHandlerController>(x => x.GoAfter(null)).FirstCall().Next.ShouldBeOfType
                <ContinuationNode>();
        }
    }
}