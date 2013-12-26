using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class FilterAttributeTester
    {
        [Test]
        public void one_method_is_valid()
        {
            FilterAttribute.DetermineMethod(typeof (ValidFilter))
                           .Name.ShouldEqual("Filter");
        }

        [Test]
        public void no_public_methods_are_invalid()
        {
            Exception<InvalidActionFilterTypeException>.ShouldBeThrownBy(() => {
                FilterAttribute.DetermineMethod(typeof (InvalidFilter2));
            });
        }

        [Test]
        public void multiple_public_methods_are_invalid()
        {
            Exception<InvalidActionFilterTypeException>.ShouldBeThrownBy(() =>
            {
                FilterAttribute.DetermineMethod(typeof(InvalidFilter));
            });
        }

        [Test]
        public void private_methods_are_invalid()
        {
            Exception<InvalidActionFilterTypeException>.ShouldBeThrownBy(() =>
            {
                FilterAttribute.DetermineMethod(typeof(InvalidFilter3));
            });
        }

        [Test]
        public void adds_the_filter_before_the_action_call()
        {
            var graph = BehaviorGraph.BuildFrom(x => {
                x.Actions.IncludeType<FilterTargetEndpoint>();
            });

            var chain = graph.BehaviorFor<FilterTargetEndpoint>(x => x.get_hello());

            var filter = chain.OfType<ActionFilter>().Single();
            filter.HandlerType.ShouldEqual(typeof (ValidFilter));
            filter.Method.Name.ShouldEqual("Filter");

            filter.Next.ShouldBeOfType<ContinuationNode>()
                .Next.ShouldBeOfType<ActionCall>();
        }
    }

    // SAMPLE: filter-attribute-usage
    public class FilterTargetEndpoint
    {
        [Filter(typeof(ValidFilter))]
        public string get_hello()
        {
            return "Hello";
        }
    }

    // Note that the ValidFilter only has one method
    // that returns a FubuContinuation
    public class ValidFilter
    {
        public FubuContinuation Filter()
        {
            return FubuContinuation.NextBehavior();
        }
    }
    // ENDSAMPLE

    public class InvalidFilter
    {
        public FubuContinuation Filter()
        {
            return FubuContinuation.NextBehavior();
        }

        public FubuContinuation Filter2()
        {
            return FubuContinuation.NextBehavior();
        }
    }

    public class InvalidFilter2
    {
        
    }

    public class InvalidFilter3
    {
        private FubuContinuation Filter()
        {
            return FubuContinuation.NextBehavior();
        }
    }
}