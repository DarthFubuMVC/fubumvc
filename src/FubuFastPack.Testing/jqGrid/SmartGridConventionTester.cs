using FubuFastPack.JqGrid;
using FubuFastPack.Testing.Security;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;
using FubuMVC.Tests.Behaviors;
using NUnit.Framework;
using FubuMVC.Tests;
using System.Linq;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class SmartGridConventionTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x => x.ApplySmartGridConventions(o =>
            {
                o.ToAssemblyContainingType<SmartGridConventionTester>();
            }));

            theGraph = registry.BuildGraph();
        }

        [Test]
        public void should_have_behavior_chains_for_both_grids_in_this_assembly()
        {
            theGraph.BehaviorFor(typeof (GridRequest<Fake1Grid>)).ShouldNotBeNull();
            theGraph.BehaviorFor(typeof (GridRequest<Fake2Grid>)).ShouldNotBeNull();
        }

        [Test]
        public void registers_an_action_call_for_smart_grid_controller_data()
        {
            var call = theGraph.BehaviorFor(typeof (GridRequest<Fake1Grid>)).FirstCall();
            call.HandlerType.ShouldEqual(typeof (SmartGridHarness<Fake1Grid>));
            call.Method.Name.ShouldEqual("Data");
        }

        [Test]
        public void use_the_grid_name_in_the_route()
        {
            theGraph.BehaviorFor<SmartGridHarness<Fake1Grid>>(x => x.Data(null))
                .RoutePattern.ShouldEqual("_griddata/fake1");
        }

        [Test]
        public void should_pull_the_authorization_rules_for_the_grid_to_the_behavior_chain()
        {
            theGraph.BehaviorFor(typeof (GridRequest<Fake1Grid>)).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("Role1", "Role2");
        }

        [Test]
        public void should_put_a_json_output_node_at_the_end()
        {
            theGraph.BehaviorFor<SmartGridHarness<Fake1Grid>>(x => x.Data(null)).Any(x => x is RenderJsonNode)
                .ShouldBeTrue();
        }
    }

    [AllowRole("Role1", "Role2")]
    public class Fake1Grid : RepositoryGrid<Case>
    {
        
    }

    [AllowRole("Role3")]
    public class Fake2Grid : ProjectionGrid<Case>
    {
        
    }
}