using System;
using System.Linq;
using FubuCore.DependencyAnalysis;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.DependencyAnalysis
{
    [TestFixture]
    public class Given_a_well_ordered_graph
    {
        private DependencyGraph<Bottle> _tree;

        [SetUp]
        public void SetUp()
        {
            _tree = new DependencyGraph<Bottle>(bot => bot.Name, bot => bot.Dependencies);


            var a = new Bottle("a");
            a.AddDependency("d");

            var b = new Bottle("b");
            b.AddDependency("a");
            b.AddDependency("c");

            var c = new Bottle("c");
            c.AddDependency("a");

            var d = new Bottle("d");

            _tree.RegisterItem(b);
            _tree.RegisterItem(c);
            _tree.RegisterItem(a);
            _tree.RegisterItem(d);
        }

        [Test]
        public void should_not_be_missing_dependencies()
        {
            _tree.HasMissingDependencies().ShouldBeFalse();
        }

        [Test]
        public void should_be_in_the_order()
        {
            _tree.GetLoadOrder().ShouldHaveTheSameElementsAs("d","a","c","b");
        }

        [Test]
        public void should_not_have_cycles()
        {
            _tree.HasCycles().ShouldBeFalse();    
        }
    }

    [TestFixture]
    public class Given_a_graph_missing_a_dependency
    {
        private DependencyGraph<Bottle> _tree;

        [SetUp]
        public void SetUp()
        {
            _tree = new DependencyGraph<Bottle>(bot => bot.Name, bot => bot.Dependencies);


            var a = new Bottle("a");
            a.AddDependency("d");

            var b = new Bottle("b");
            b.AddDependency("a");
            b.AddDependency("c");

            var c = new Bottle("c");
            c.AddDependency("a");


            _tree.RegisterItem(b);
            _tree.RegisterItem(c);
            _tree.RegisterItem(a);
        }

        [Test]
        public void should_not_be_missing_dependencies()
        {
            _tree.HasMissingDependencies().ShouldBeTrue();
        }

        [Test]
        public void the_missing_dependency()
        {
            _tree.MissingDependencies().ShouldHaveTheSameElementsAs("d");
        }

        [Test]
        public void should_be_in_the_order()
        {
            _tree.GetLoadOrder().ShouldHaveTheSameElementsAs("d", "a", "c", "b");
        }

        [Test]
        public void should_not_have_cycles()
        {
            _tree.HasCycles().ShouldBeFalse();
        }
    }

    [TestFixture]
    public class Given_a_graph_with_a_cycle
    {
        private DependencyGraph<Bottle> _tree;

        [SetUp]
        public void SetUp()
        {
            _tree = new DependencyGraph<Bottle>(bot => bot.Name, bot => bot.Dependencies);


            var a = new Bottle("a");
            a.AddDependency("d");

            var b = new Bottle("b");
            b.AddDependency("a");
            b.AddDependency("c");

            var c = new Bottle("c");
            c.AddDependency("b");

            var d = new Bottle("d");

            _tree.RegisterItem(b);
            _tree.RegisterItem(c);
            _tree.RegisterItem(a);
            _tree.RegisterItem(d);
        }

        [Test]
        public void should_not_be_missing_dependencies()
        {
            _tree.HasMissingDependencies().ShouldBeFalse();
        }

        [Test]
        public void should_be_in_the_order()
        {
            typeof (InvalidOperationException).ShouldBeThrownBy(()=>
                {
                    var order = _tree.GetLoadOrder().ToList();
                });
        }

        [Test]
        public void should_have_cycles()
        {
            _tree.HasCycles().ShouldBeTrue();
        }
    }
}