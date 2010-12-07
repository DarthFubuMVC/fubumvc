using System.Linq;
using FubuCore;
using FubuMVC.UI.Scripts;
using FubuMVC.UI.Scripts.Registration;
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

        [Test]
        public void should_return_script_with_ordered_dependencies_for_multiple_generations()
        {
            var script = _graph.RegisterScript("complex-test", "complex-test.js");
            gatherDependencies(script, 3, 0);

            var scripts = _graph.GetScript("complex-test");
            scripts.ShouldNotBeEmpty();
        }

        [Test]
        public void should_resolved_proper_order_when_dependencies_shared_dependencies()
        {
            var script = _graph.RegisterScript("test", "test.js");
            var a = _graph.RegisterScript("{0}-dependency-a-{1}".ToFormat(script.Name, 1), "{0}-dependency-a-{1}.js".ToFormat(script.Name, 1));
            var b = _graph.RegisterScript("{0}-dependency-b-{1}".ToFormat(script.Name, 1), "{0}-dependency-b-{1}.js".ToFormat(script.Name, 1));
            var c = _graph.RegisterScript("{0}-dependency-c-{1}".ToFormat(script.Name, 1), "{0}-dependency-c-{1}.js".ToFormat(script.Name, 1));
            var d = _graph.RegisterScript("{0}-dependency-d-{1}".ToFormat(script.Name, 1), "{0}-dependency-d-{1}.js".ToFormat(script.Name, 1));

            a.AddDependency(b);
            b.AddDependency(d);
            c.AddDependency(b);

            script.AddDependency(a);
            script.AddDependency(c);

            var scripts = _graph.GetScript("test").ToArray();

            scripts.ShouldHaveCount(5);
            scripts[0].ShouldEqual(d);
            scripts[1].ShouldEqual(b);
            scripts[2].ShouldEqual(a);
            scripts[3].ShouldEqual(c);
            scripts[4].ShouldEqual(script);
        }

        private void gatherDependencies(Script script, int limit, int index)
        {
            if(index + 1 == limit)
            {
                return;
            }

            var x = _graph.RegisterScript("{0}-dependency-x-{1}".ToFormat(script.Name, index), "{0}-dependency-x-{1}.js".ToFormat(script.Name, index));
            var y = _graph.RegisterScript("{0}-dependency-y-{1}".ToFormat(script.Name, index), "{0}-dependency-y-{1}.js".ToFormat(script.Name, index));

            gatherDependencies(x, limit, index + 1);
            gatherDependencies(y, limit, index + 1);

            script.AddDependency(x);
            script.AddDependency(y);
        }
    }
}