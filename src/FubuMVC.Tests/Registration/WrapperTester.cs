using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.StructureMap;
using Shouldly;
using Xunit;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.Registration
{
    
    public class WrapperTester
    {
        private Wrapper _wrapper = new Wrapper(typeof(NulloBehavior));

        [Fact]
        public void can_get_the_concrete_behavior_type()
        {
            _wrapper.BehaviorType.ShouldBe(typeof (NulloBehavior));

            _wrapper.BehaviorType.ShouldBe(typeof (NulloBehavior));
        }

        [Fact]
        public void build_an_object_def_for_the_type()
        {
            var def = _wrapper.As<IContainerModel>().ToInstance().As<IConfiguredInstance>();
            def.Dependencies.Count().ShouldBe(0);

            def.PluggedType.ShouldBe(typeof (NulloBehavior));
        }

        [Fact]
        public void ctor_blows_up_if_the_type_is_not_an_action_behavior()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => { new Wrapper(GetType()); });
        }

        [Fact]
        public void put_a_dependency_into_the_object_def_for_the_inner_behavior()
        {
            _wrapper.AddAfter(Wrapper.For<FakeBehavior>());
            var def = _wrapper.As<IContainerModel>().ToInstance().As<IConfiguredInstance>();

            def.Dependencies.Count().ShouldBe(1);

            def.FindDependencyDefinitionFor<IActionBehavior>().ReturnedType
                .ShouldBe(typeof (FakeBehavior));

      
        }

        [Fact]
        public void the_object_def_name_is_copied_from_the_unique_id_of_the_wrapper()
        {
            _wrapper.As<IContainerModel>().ToInstance().Name.ShouldBe(
                _wrapper.UniqueId.ToString());
        }
    }
}