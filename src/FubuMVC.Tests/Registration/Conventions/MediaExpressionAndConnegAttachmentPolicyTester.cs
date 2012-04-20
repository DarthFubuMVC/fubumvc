using System;
using System.Linq.Expressions;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class MediaExpressionAndConnegAttachmentPolicyTester
    {
        private FubuRegistry theFubuRegistry;
        private Lazy<BehaviorGraph> theGraph;


        [SetUp]
        public void SetUp()
        {
            theFubuRegistry = new FubuRegistry();
            theFubuRegistry.Applies.ToThisAssembly();
            theFubuRegistry.Actions.IncludeType<Controller1>();

            theGraph = new Lazy<BehaviorGraph>(() => theFubuRegistry.BuildGraph());
        } 

        private BehaviorChain chainFor(Expression<Func<Controller1, object>> expression)
        {
            return theGraph.Value.BehaviorFor(expression);
        }

        [Test]
        public void apply_content_by_action()
        {
            theFubuRegistry.Media.ApplyContentNegotiationToActions(x => x.OutputType() == typeof (ViewModel3));

            chainFor(x => x.C()).HasConnegOutput().ShouldBeTrue();
            chainFor(x => x.D()).HasConnegOutput().ShouldBeTrue();
            chainFor(x => x.E()).HasConnegOutput().ShouldBeTrue();
        
            chainFor(x => x.A()).HasConnegOutput().ShouldBeFalse();
            chainFor(x => x.B()).HasConnegOutput().ShouldBeFalse();
        }

        [Test]
        public void apply_content_by_looking_at_a_chain()
        {
            theFubuRegistry.Media.ApplyContentNegotiationTo(chain => chain.FirstCall().Method.Name == "A");

            chainFor(x => x.A()).HasConnegOutput().ShouldBeTrue();

            chainFor(x => x.B()).HasConnegOutput().ShouldBeFalse();
            chainFor(x => x.C()).HasConnegOutput().ShouldBeFalse();
            chainFor(x => x.D()).HasConnegOutput().ShouldBeFalse();
            chainFor(x => x.E()).HasConnegOutput().ShouldBeFalse();
        }
    }

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