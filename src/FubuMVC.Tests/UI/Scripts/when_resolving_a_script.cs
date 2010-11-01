using System.Linq;
using FubuMVC.UI.Scripts;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Scripts
{
    [TestFixture]
    public class when_resolving_a_script
    {
        private ScriptGraph _graph;

        [SetUp]
        public void BeforeEach()
        {
            _graph = new ScriptGraph();
        }

        [Test]
        public void should_return_single_script_if_there_are_no_dependencies()
        {
            var script = _graph.RegisterScript("simple-script", "simple-script.js");

            var scripts = _graph.GetScript("simple-script");
            scripts.ShouldHaveCount(1);
            scripts.ShouldContain(script);
        }

        [Test]
        public void should_return_script_with_ordered_dependencies_for_single_generation_of_dependencies()
        {
            var script = _graph.RegisterScript("script-with-dependencies", "script-with-dependencies.js");
            var dependency1 = _graph.RegisterScript("dependency-1", "dependency-1.js");
            var dependency2 = _graph.RegisterScript("dependency-2", "dependency-2.js");

            script.AddDependency(dependency1);
            script.AddDependency(dependency2);

            var scripts = _graph.GetScript(script.Name).ToArray();

            scripts.ShouldHaveCount(3);
            scripts[0].ShouldEqual(dependency1);
            scripts[1].ShouldEqual(dependency2);
            scripts[2].ShouldEqual(script);
        }
    }
}