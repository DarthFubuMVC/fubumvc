using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Tests.Behaviors;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class WrapperTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _wrapper = new Wrapper(typeof (NulloBehavior));
        }

        #endregion

        private Wrapper _wrapper;

        [Test]
        public void build_an_object_def_for_the_type()
        {
            ObjectDef def = _wrapper.ToObjectDef();
            def.Dependencies.Count.ShouldEqual(0);

            def.Type.ShouldEqual(typeof (NulloBehavior));
        }

        [Test]
        public void put_a_dependency_into_the_object_def_for_the_inner_behavior()
        {
            _wrapper.InsertDirectlyAfter(new RenderJsonNode(typeof (Output)));
            ObjectDef def = _wrapper.ToObjectDef();

            def.Dependencies.Count.ShouldEqual(1);

            var dependency = def.Dependencies.First().ShouldBeOfType<ConfiguredDependency>();
            dependency.DependencyType.ShouldEqual(typeof (IActionBehavior));
            dependency.Definition.Type.ShouldEqual(typeof (RenderJsonBehavior<Output>));
        }

        [Test]
        public void the_object_def_name_is_copied_from_the_unique_id_of_the_wrapper()
        {
            _wrapper.ToObjectDef().Name.ShouldEqual(_wrapper.UniqueId.ToString());
        }
    }
}