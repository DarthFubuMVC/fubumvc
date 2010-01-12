using System;
using System.Reflection;
using FubuMVC.Core.Util;
using NUnit.Framework;

namespace FubuMVC.Tests.Util
{
    [TestFixture]
    public class ReflectionHelperTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            PropertyInfo top = ReflectionHelper.GetProperty<Target>(x => x.Child);
            PropertyInfo second = ReflectionHelper.GetProperty<ChildTarget>(x => x.GrandChild);
            PropertyInfo birthday = ReflectionHelper.GetProperty<GrandChildTarget>(x => x.BirthDay);

            _chain = new PropertyChain(new[] {top, second, birthday});
        }

        #endregion

        private PropertyChain _chain;

        public class Target
        {
            public string Name { get; set; }
            public ChildTarget Child { get; set; }
        }

        public class ChildTarget
        {
            public int Age { get; set; }
            public GrandChildTarget GrandChild { get; set; }
        }

        public class GrandChildTarget
        {
            public DateTime BirthDay { get; set; }
            public string Name { get; set; }
        }

        public class SomeClass
        {
            public object DoSomething()
            {
                return null;
            }
        }

        [Test]
        public void CreateAPropertyChainFromReflectionHelper()
        {
            Accessor accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChild.BirthDay);
            var target = new Target
            {
                Child = new ChildTarget
                {
                    GrandChild = new GrandChildTarget
                    {
                        BirthDay = DateTime.Today
                    }
                }
            };

            accessor.GetValue(target).ShouldEqual(DateTime.Today);
        }

        [Test]
        public void DeclaringType_of_a_property_chain_is_the_type_of_the_leftmost_object()
        {
            Accessor accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChild.BirthDay);
            accessor.ShouldBeOfType<PropertyChain>().DeclaringType.ShouldEqual(typeof (Target));
        }

        [Test]
        public void DeclaringType_of_a_single_property_is_type_of_the_object_containing_the_property()
        {
            Accessor accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child);
            accessor.ShouldBeOfType<SingleProperty>().DeclaringType.ShouldEqual(typeof (Target));
        }

        [Test]
        public void equals_for_a_single_property()
        {
            SingleProperty prop1 = SingleProperty.Build<Target>(x => x.Name);
            SingleProperty prop2 = SingleProperty.Build<Target>(x => x.Name);
            SingleProperty prop3 = SingleProperty.Build<Target>(x => x.Child);

            prop1.ShouldEqual(prop2);
            prop1.ShouldNotEqual(prop3);
        }

        [Test]
        public void GetFieldNameFromSingleProperty()
        {
            SingleProperty property = SingleProperty.Build<Target>(x => x.Name);
            ((object) property.FieldName).ShouldEqual("Name");
        }

        [Test]
        public void GetNameFromSingleProperty()
        {
            SingleProperty property = SingleProperty.Build<Target>(x => x.Name);
            property.Name.ShouldEqual("Name");
        }

        [Test]
        public void GetValueFromSingleProperty()
        {
            var target = new Target
            {
                Name = "Jeremy"
            };
            SingleProperty property = SingleProperty.Build<Target>(x => x.Name);
            property.GetValue(target).ShouldEqual("Jeremy");
        }

        [Test]
        public void property_chain_equals()
        {
            Accessor chain1 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay);
            Accessor chain2 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay);
            Accessor chain3 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild);
            Accessor chain4 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.Name);

            chain1.ShouldEqual(chain2);
            chain1.ShouldNotEqual(chain3);
            chain1.ShouldNotEqual(chain4);
        }

        [Test]
        public void PropertyChain_can_get_the_name()
        {
            ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay).Name.ShouldEqual(
                "ChildGrandChildBirthDay");
        }

        [Test]
        public void PropertyChainCanGetPropertyHappyPath()
        {
            var target = new Target
            {
                Child = new ChildTarget
                {
                    GrandChild = new GrandChildTarget
                    {
                        BirthDay = DateTime.Today
                    }
                }
            };
            _chain.GetValue(target).ShouldEqual(DateTime.Today);
        }


        [Test]
        public void PropertyChainCanSetPRopertyHappyPath()
        {
            var target = new Target
            {
                Child = new ChildTarget
                {
                    GrandChild = new GrandChildTarget
                    {
                        BirthDay = DateTime.Today
                    }
                }
            };
            _chain.SetValue(target, DateTime.Today.AddDays(1));

            target.Child.GrandChild.BirthDay.ShouldEqual(DateTime.Today.AddDays(1));
        }

        [Test]
        public void PropertyChainGetPropertyReturnsNullForSadPath()
        {
            var target = new Target
            {
                Child = new ChildTarget()
            };
            _chain.GetValue(target).ShouldBeNull();
        }

        [Test]
        public void PropertyChainReturnsInnerMostFieldName()
        {
            _chain.FieldName.ShouldEqual("BirthDay");
        }

        [Test]
        public void PropertyChainReturnsInnerMostPropertyType()
        {
            _chain.PropertyType.ShouldEqual(typeof (DateTime));
        }

        [Test]
        public void SetValueFromSingleProperty()
        {
            var target = new Target
            {
                Name = "Jeremy"
            };
            SingleProperty property = SingleProperty.Build<Target>(x => x.Name);
            property.SetValue(target, "Justin");

            target.Name.ShouldEqual("Justin");
        }

        [Test]
        public void Try_to_fetch_a_method()
        {
            MethodInfo method = ReflectionHelper.GetMethod<SomeClass>(s => s.DoSomething());
            method.Name.ShouldEqual("DoSomething");
        }

        [Test]
        public void TryingToCallSetDoesNotBlowUpIfTheIntermediateChildrenAreNotThere()
        {
            var target = new Target
            {
                Child = new ChildTarget()
            };
            _chain.SetValue(target, DateTime.Today.AddDays(4));
        }
    }
}