using System;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Ajax
{
    
    public class AjaxContinuationPolicyIntegratedTester
    {
        public AjaxContinuationPolicyIntegratedTester()
        {
            theGraph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<Controller1>();
            });
        }


        private BehaviorGraph theGraph;

        private BehaviorChain chainFor(Expression<Action<Controller1>> method)
        {
            return theGraph.ChainFor(method);
        }


        public class Input
        {
        }

        public class Output
        {
        }

        public class Controller1
        {
            public Output NoContinuation(Input input)
            {
                return null;
            }

            public AjaxContinuation BasicContinuation(Input input)
            {
                return null;
            }

            public MySpecialContinuation SpecialContinuation(Input input)
            {
                return null;
            }
        }

        public class MySpecialContinuation : AjaxContinuation
        {
        }


        [Fact]
        public void should_be_a_behavior_on_actions_that_return_a_subclass_of_AjaxContinuation()
        {
            var outputNode = chainFor(x => x.SpecialContinuation(null))
                .Output;


            outputNode.Media().OfType<AjaxContinuationWriter<MySpecialContinuation>>()
                .Any()
                .ShouldBeTrue();
        }

        [Fact]
        public void should_be_a_behavior_on_actions_that_return_the_AjaxContinuation()
        {
            chainFor(x => x.BasicContinuation(null)).Output.Media().OfType<AjaxContinuationWriter<AjaxContinuation>>()
                .Any()
                .ShouldBeTrue();
        }

        [Fact]
        public void should_have_a_conneg_input_node_with_json_or_http_post_input()
        {
            var connegInput = chainFor(x => x.BasicContinuation(null)).Input;
            connegInput.CanRead(MimeType.HttpFormMimetype).ShouldBeTrue();
            connegInput.CanRead(MimeType.Json).ShouldBeTrue();
        }

    }
}