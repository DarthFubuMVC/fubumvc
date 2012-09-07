using System.Linq;
using Bottles;
using Bottles.Configuration;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class BottleConfigurationDefTester
    {
        private BottleConfigurationDef theConfigurationHelper;
        private string theProvenance;

        [SetUp]
        public void SetUp()
        {
            theProvenance = GetType().Namespace;
            theConfigurationHelper = new BottleConfigurationDef(theProvenance);
        }

        [Test]
        public void sets_the_string_dependency()
        {
            theObjectDef
                .Dependencies
                .OfType<ValueDependency>()
                .Single()
                .Value.ShouldEqual(theProvenance);
        }

        [Test]
        public void adds_rule_types_to_the_list_dependency()
        {
            theConfigurationHelper.AddRule<FakeRule>();

            theObjectDef
                .Dependencies
                .OfType<ListDependency>()
                .Single()
                .Items
                .First()
                .Type.ShouldEqual(typeof (FakeRule));
        }

        [Test]
        public void adds_rule_values_to_the_list_dependency()
        {
            var rule = new FakeRule();
            theConfigurationHelper.AddRule(rule);

            theObjectDef
                .Dependencies
                .OfType<ListDependency>()
                .Single()
                .Items
                .First()
                .Value.ShouldBeTheSameAs(rule);
        }

        private ObjectDef theObjectDef
        {
            get { return theServiceGraph.ServicesFor<IActivator>().Single(x => x.Type == typeof(AssertBottleConfiguration)); }
        }

        private ServiceGraph theServiceGraph
        {
            get
            {
                var graph = BehaviorGraph.BuildFrom(new FubuRegistry()).Services;
                theConfigurationHelper.As<IServiceGraphAlteration>().Alter(graph);

                return graph;
            }
        }

        public class FakeRule : IBottleConfigurationRule 
        {
            public void Evaluate(BottleConfiguration configuration)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}