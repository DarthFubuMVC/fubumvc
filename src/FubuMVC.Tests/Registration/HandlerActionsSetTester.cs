using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class HandlerActionsSetTester
    {
        private BehaviorGraph theGraph;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x =>
            {
                x.Actions.IncludeClassesSuffixedWithController();
            });

            theGraph = BehaviorGraph.BuildFrom(registry);
        }

        [Test]
        public void find_a_handler_action_set_for_a_type()
        {
            var handlerActions = theGraph.ActionsForHandler<AwesomeController>();

            handlerActions.Each(x => x.HandlerType.ShouldEqual(typeof (AwesomeController)));
            handlerActions.Select(x => x.Method.Name)
                .ShouldHaveTheSameElementsAs("M1", "M2", "Different");
        }

        [Test]
        public void find_action_in_a_set_by_method_name()
        {
            var handlerActions = theGraph.ActionsForHandler<AwesomeController>();
            handlerActions.ByName("M1").Method.Name.ShouldEqual("M1");
        }

        [Test]
        public void find_actions_that_start_with_prefix()
        {
            var handlerActions = theGraph.ActionsForHandler<AwesomeController>();
            handlerActions.StartingWith("M").Select(x => x.Method.Name)
                .ShouldHaveTheSameElementsAs("M1", "M2");
        }


        [Test]
        public void find_actions_by_output()
        {
            var handlerActions = theGraph.ActionsForHandler<EvenMoreAwesomeController>();
            handlerActions.ForOutput(t => t == typeof(Model2)).Select(x => x.Method.Name)
                .ShouldHaveTheSameElementsAs("Out1", "Out2");
        }

        [Test]
        public void find_handler_sets_by_a_criteria_on_handler_type()
        {
            var handlerSets = theGraph.HandlerSetsFor(type => type.Name.Contains("Awesome"));
            handlerSets.Select(x => x.HandlerType.Name)
                .ShouldHaveTheSameElementsAs("AwesomeController", "MoreAwesomeController", "EvenMoreAwesomeController");
        }

    }

    public class AwesomeController
    {
        public void M1(Model1 input){}
        public Model2 M2(Model1 input)
        {
            return null;
        }

        public void Different(Model1 input) { }
    }

    public class MoreAwesomeController
    {
        public void M1(Model1 input) { }
        public Model2 M2(Model1 input)
        {
            return null;
        }

        public void Different(Model1 input) { }
    }

    public class EvenMoreAwesomeController
    {
        public void M1(Model1 input) { }
        public void M2(Model1 input) { }
        public Model2 Out1(Model1 input)
        {
            return null;
        }

        public Model2 Out2(Model1 input)
        {
            return null;
        }
    }
}