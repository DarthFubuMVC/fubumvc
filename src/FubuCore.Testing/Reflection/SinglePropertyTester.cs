using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection
{
    [TestFixture]
    public class SinglePropertyTester
    {
        public class Target
        {
            public string Name { get; set; }
            public ChildTarget Child { get; set; }
            public int Number { get; set; }
        }

        public class ChildTarget
        {
            public int Age { get; set; }
            public GrandChildTarget GrandChild { get; set; }
            public GrandChildTarget SecondGrandChild { get; set; }
        }

        public class GrandChildTarget
        {
            public DateTime BirthDay { get; set; }
            public string Name { get; set; }
        }

        public class HierarchicalTarget
        {
            public Target Child { get; set; }
        }

        [Test]
        public void DeclaringType_of_a_single_property_is_type_of_the_object_containing_the_property()
        {
            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child);
            accessor.ShouldBeOfType<SingleProperty>().DeclaringType.ShouldEqual(typeof (Target));
        }

        [Test]
        public void GetFieldNameFromSingleProperty()
        {
            var property = SingleProperty.Build<Target>(x => x.Name);
            ((object) property.FieldName).ShouldEqual("Name");
        }

        [Test]
        public void GetNameFromSingleProperty()
        {
            var property = SingleProperty.Build<Target>(x => x.Name);
            property.Name.ShouldEqual("Name");
        }

        [Test]
        public void GetValueFromSingleProperty()
        {
            var target = new Target{
                Name = "Jeremy"
            };
            var property = SingleProperty.Build<Target>(x => x.Name);
            property.GetValue(target).ShouldEqual("Jeremy");
        }

        [Test]
        public void SetValueFromSingleProperty()
        {
            var target = new Target{
                Name = "Jeremy"
            };
            var property = SingleProperty.Build<Target>(x => x.Name);
            property.SetValue(target, "Justin");

            target.Name.ShouldEqual("Justin");
        }

        [Test]
        public void build_single_property()
        {
            var prop1 = SingleProperty.Build<Target>("Child");
            var prop2 = SingleProperty.Build<Target>(x => x.Child);
            prop1.ShouldEqual(prop2);
            prop1.Name.ShouldEqual("Child");
            prop1.OwnerType.ShouldEqual(typeof (Target));
        }

        [Test]
        public void equals_for_a_single_property()
        {
            var prop1 = SingleProperty.Build<Target>(x => x.Name);
            var prop2 = SingleProperty.Build<Target>(x => x.Name);
            var prop3 = SingleProperty.Build<Target>(x => x.Child);

            prop1.ShouldEqual(prop2);
            prop1.ShouldNotEqual(prop3);
            prop1.Equals(null).ShouldBeFalse();
            prop1.Equals(prop1).ShouldBeTrue();
            prop1.ShouldEqual(prop1);
            prop1.Equals((object) null).ShouldBeFalse();
            prop1.Equals(42).ShouldBeFalse();
        }

        [Test]
        public void get_expression_from_accessor()
        {
            var target = new Target{
                Name = "Chad"
            };

            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Name);

            var expression = accessor.ToExpression<Target>();
            expression.Compile()(target).ShouldEqual("Chad");
        }


        [Test]
        public void get_expression_from_accessor_for_a_number()
        {
            var target = new Target{
                Number = 3
            };

            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Number);

            var expression = accessor.ToExpression<Target>();
            expression.Compile()(target).ShouldEqual(3);
        }

        [Test]
        public void prepend_should_return_a_property_chain()
        {
            var accessor = ReflectionHelper.GetAccessor<Target>(x => x.Name);
            var property = ReflectionHelper.GetProperty<HierarchicalTarget>(x => x.Child);

            var prependedAccessor = accessor.Prepend(property);
            prependedAccessor.ShouldBeOfType<PropertyChain>();
            prependedAccessor.PropertyNames.ShouldHaveTheSameElementsAs("Child", "Name");

            var target = new HierarchicalTarget{
                Child = new Target{
                    Name = "Jeremy"
                }
            };

            prependedAccessor.GetValue(target).ShouldEqual("Jeremy");
        }

        [Test]
        public void singleProperty_can_get_child_accessor()
        {
            var expected = ReflectionHelper.GetAccessor<ChildTarget>(c => c.GrandChild.Name);
            SingleProperty.Build<Target>(t => t.Child.GrandChild).
                GetChildAccessor<GrandChildTarget>(t => t.Name).ShouldEqual(expected);
        }

        [Test]
        public void singleProperty_property_names_contains_single_value()
        {
            SingleProperty.Build<Target>(t => t.Child.GrandChild.Name).PropertyNames.
                ShouldHaveCount(1).ShouldContain("Name");
        }
    }
}