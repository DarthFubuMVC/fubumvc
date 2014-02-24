using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuTestingSupport;
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
        public void can_get_the_concrete_behavior_type()
        {
            _wrapper.BehaviorType.ShouldEqual(typeof (NulloBehavior));

            _wrapper.BehaviorType.ShouldEqual(typeof(NulloBehavior));
        }

        [Test]
        public void build_an_object_def_for_the_type()
        {
            var def = _wrapper.As<IContainerModel>().ToObjectDef();
            def.Dependencies.Count().ShouldEqual(0);

            def.Type.ShouldEqual(typeof (NulloBehavior));
        }

        [Test]
        public void ctor_blows_up_if_the_type_is_not_an_action_behavior()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => { new Wrapper(GetType()); });
        }

        [Test]
        public void put_a_dependency_into_the_object_def_for_the_inner_behavior()
        {
            _wrapper.AddAfter(Wrapper.For<FakeBehavior>());
            var def = _wrapper.As<IContainerModel>().ToObjectDef();

            def.Dependencies.Count().ShouldEqual(1);

            var dependency = def.Dependencies.First().ShouldBeOfType<ConfiguredDependency>();
            dependency.DependencyType.ShouldEqual(typeof (IActionBehavior));
            dependency.Definition.Type.ShouldEqual(typeof (FakeBehavior));
        }

        [Test]
        public void the_object_def_name_is_copied_from_the_unique_id_of_the_wrapper()
        {
            _wrapper.As<IContainerModel>().ToObjectDef().Name.ShouldEqual(
                _wrapper.UniqueId.ToString());
        }
    }
}