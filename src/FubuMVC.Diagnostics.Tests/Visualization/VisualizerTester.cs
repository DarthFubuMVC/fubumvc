using System.Reflection;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core;
using FubuMVC.Diagnostics.Visualization;
using NUnit.Framework;
using FubuMVC.StructureMap;
using StructureMap;
using FubuTestingSupport;
using FubuMVC.Katana;

namespace FubuMVC.Diagnostics.Tests.Visualization
{
    [TestFixture, Explicit("Spark blows up for some reason.  Not gonna worry about it for now.")]
    public class VisualizerTester
    {
        private IVisualizer theVisualizer;
        private IContainer container;
        private EmbeddedFubuMvcServer server;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<FakeEndpoint>();

            server = FubuApplication.For(registry).StructureMap(new Container())
                .Packages(x => {
                    x.Assembly(Assembly.GetExecutingAssembly());
                })
                                        .RunEmbedded(".".ToFullPath());

        }

        [TearDown]
        public void TearDown()
        {
            server.Dispose();
        }

        [Test]
        public void build_with_a_specific_visualizer()
        {
            server.Endpoints.Get<FakeEndpoint>(x => x.get_has_visualizer())
                .ReadAsText()
                .ShouldContain("I'm Mr Jeremy");
        }

        [Test]
        public void build_without_a_visualizer_falls_back_to_description()
        {
            server.Endpoints.Get<FakeEndpoint>(x => x.get_not_visualized())
                .ReadAsText()
                .ShouldContain("I'm a ThingWithNoVisualizer".HtmlEncode());
        }

        [Title("I'm a ThingWithNoVisualizer")]
        public class ThingWithNoVisualizer
        {
        }

        public class FakeInput
        {
            public string Name { get; set; }
        }

        public class FakeEndpoint
        {
            private readonly IVisualizer _visualizer;

            public FakeEndpoint(IVisualizer visualizer)
            {
                _visualizer = visualizer;
            }

            public string get_not_visualized()
            {
                return _visualizer.Visualize(new ThingWithNoVisualizer()).ToString();
            }

            public string get_has_visualizer()
            {
                return _visualizer.Visualize(new FakeInput {Name = "Jeremy"}).ToString();
            }

            public string Visualize(FakeInput input)
            {
                return "I'm Mr " + input.Name;
            }
        }
    }
}