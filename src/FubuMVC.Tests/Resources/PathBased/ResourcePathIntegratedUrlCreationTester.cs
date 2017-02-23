using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.PathBased;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Resources.PathBased
{
	
    public class ResourcePathIntegratedUrlCreationTester
    {
        public class Controller1
        {
            public string get_resource(ResourcePath path)
            {
                return null;
            }

			public string get_special(SpecialResourcePath path)
            {
                return null;
            }

            public string SayHello()
            {
                return null;
            }
        }

        public class SpecialResourcePath : ResourcePath
        {
            public SpecialResourcePath(string path) : base(path)
            {
            }
        }

        [Fact]
        public void uses_the_resource_path_to_do_its_job()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            BehaviorGraph.BuildFrom(registry).ChainFor<Controller1>(x => x.get_resource(null)).As<RoutedChain>().Route.CreateUrlFromInput(
                new ResourcePath("something/else"))
                .ShouldBe("resource/something/else");
        }

        [Fact]
        public void should_append_the_url_suffix_onto_each_appropriate_route()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            var graph = BehaviorGraph.BuildFrom(registry);

            graph.ChainFor<Controller1>(x => x.get_resource(null))
                .As<RoutedChain>()
                .Route.Pattern.ShouldBe(
                    "resource/{Part0}/{Part1}/{Part2}/{Part3}/{Part4}/{Part5}/{Part6}/{Part7}/{Part8}/{Part9}");

            graph.ChainFor<Controller1>(x => x.get_special(null))
                .As<RoutedChain>()
                .Route.Pattern.ShouldBe(
                    "special/{Part0}/{Part1}/{Part2}/{Part3}/{Part4}/{Part5}/{Part6}/{Part7}/{Part8}/{Part9}");

        }


    }
}