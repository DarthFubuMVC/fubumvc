using Bottles;
using FubuFastPack.JqGrid;
using FubuFastPack.Persistence;
using FubuFastPack.Testing.Security;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using StructureMap;
using FubuFastPack.StructureMap;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class SmartGridConventionTester
    {
        private BehaviorGraph theGraph;
        private FubuRegistry theRegistry;

        [SetUp]
        public void SetUp()
        {
            theRegistry = new FubuRegistry(x => x.ApplySmartGridConventions(o =>
            {
                
                o.ToAssemblyContainingType<SmartGridConventionTester>();
            }));

            // Want no types here
            theRegistry.Actions.IncludeTypes(t => false);

            theGraph = theRegistry.BuildGraph();
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
            theGraph.BehaviorFor<SmartGridHarness<Fake1Grid>>(x => x.Data(null)).GetRoutePattern().ShouldEqual("_griddata/fake1");
        }

        [Test]
        public void should_pull_the_authorization_rules_for_the_grid_to_the_behavior_chain()
        {
            theGraph.BehaviorFor(typeof (GridRequest<Fake1Grid>)).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("Role1", "Role2");
        }

        [Test]
        public void should_put_a_json_output_node_at_the_end()
        {
            theGraph.BehaviorFor<SmartGridHarness<Fake1Grid>>(x => x.Data(null)).Top.Any(x => x is RenderJsonNode)
                .ShouldBeTrue();
        }

        [Test]
        public void each_smart_grid_harness_is_registered_by_name_in_the_container()
        {
            var container = new Container(x =>
            {
                x.For<IRepository>().Use<InMemoryRepository>();
                x.For<IEntityFinder>().Use<EntityFinder>();
                x.For<IEntityFinderCache>().Use<StructureMapEntityFinderCache>();
            });

            FubuApplication.For(() => theRegistry)
                .StructureMap(() => container)
                .Packages(x => x.Assembly(typeof(ISmartGrid).Assembly))
                .Bootstrap();

            PackageRegistry.AssertNoFailures();

            container.GetInstance<ISmartGridHarness>("Fake1").ShouldBeOfType<SmartGridHarness<Fake1Grid>>();
            container.GetInstance<ISmartGridHarness>("Fake2").ShouldBeOfType<SmartGridHarness<Fake2Grid>>();
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