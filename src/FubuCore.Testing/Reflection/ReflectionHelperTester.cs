using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection
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

            _chain = new PropertyChain(new[]
            {
                new PropertyValueGetter(top),
                new PropertyValueGetter(second),
                new PropertyValueGetter(birthday),
            });
            _expression = (t => t.Child);
        }

        #endregion

        private PropertyChain _chain;
        private Expression<Func<Target, ChildTarget>> _expression;

        public class Target
        {
            public string Name { get; set; }
            public ChildTarget Child { get; set; }
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

        public class SomeClass
        {
            public object DoSomething()
            {
                return null;
            }

            public object DoSomething(int i, int j)
            {
                return null;
            }
        }

        public class ClassConstraintHolder<T> where T : class {}
        public class StructConstraintHolder<T> where T : struct {}
        public class NewConstraintHolder<T> where T : new() {}
        public class NoConstraintHolder<T> {}
        public class NoEmptyCtorHolder { public NoEmptyCtorHolder(bool ctorArg) {} }

        [Test]
        public void tell_if_type_meets_generic_constraints()
        {
            Type[] arguments = typeof (ClassConstraintHolder<>).GetGenericArguments();
            ReflectionHelper.MeetsSpecialGenericConstraints(arguments[0], typeof(int)).ShouldBeFalse();
            ReflectionHelper.MeetsSpecialGenericConstraints(arguments[0], typeof(object)).ShouldBeTrue();
            arguments = typeof (StructConstraintHolder<>).GetGenericArguments();
            ReflectionHelper.MeetsSpecialGenericConstraints(arguments[0], typeof(object)).ShouldBeFalse();
            arguments = typeof(NewConstraintHolder<>).GetGenericArguments();
            ReflectionHelper.MeetsSpecialGenericConstraints(arguments[0], typeof(NoEmptyCtorHolder)).ShouldBeFalse();
            arguments = typeof(NoConstraintHolder<>).GetGenericArguments();
            ReflectionHelper.MeetsSpecialGenericConstraints(arguments[0], typeof(object)).ShouldBeTrue();
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
        public void can_get_accessor_from_lambda_expression()
        {
            Accessor accessor = ReflectionHelper.GetAccessor((LambdaExpression)_expression);
            accessor.Name.ShouldEqual("Child");
            accessor.DeclaringType.ShouldEqual(typeof (Target));
        }

        [Test]
        public void can_get_member_expression_from_lambda()
        {
            MemberExpression memberExpression = ((LambdaExpression) _expression).GetMemberExpression(false);
            memberExpression.ToString().ShouldEqual("t.Child");
        }

        [Test]
        public void can_get_member_expression_from_convert()
        {
            Expression<Func<Target, object>> convertExpression = t => (object)t.Child;
            convertExpression.GetMemberExpression(false).ToString().ShouldEqual("t.Child");
        }

        [Test]
        public void getMemberExpression_should_throw_when_not_a_member_access()
        {
            Expression<Func<Target, object>> typeAsExpression = t => t.Child as object;
            typeof(ArgumentException).ShouldBeThrownBy(() => typeAsExpression.GetMemberExpression(true)).Message.ShouldContain("Not a member access");
        }

        [Test]
        public void DeclaringType_of_a_property_chain_is_the_type_of_the_leftmost_object()
        {
            Accessor accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child.GrandChild.BirthDay);
            accessor.ShouldBeOfType<PropertyChain>().DeclaringType.ShouldEqual(typeof (Target));
        }

        [Test]
        public void Try_to_fetch_a_method()
        {
            MethodInfo method = ReflectionHelper.GetMethod<SomeClass>(s => s.DoSomething());
            const string expected = "DoSomething";
            method.Name.ShouldEqual(expected);

            Expression<Func<object>> doSomething = () => new SomeClass().DoSomething();
            ReflectionHelper.GetMethod(doSomething).Name.ShouldEqual(expected);

            Expression doSomethingExpression = Expression.Call(Expression.Parameter(typeof (SomeClass), "s"), method);
            ReflectionHelper.GetMethod(doSomethingExpression).Name.ShouldEqual(expected);

            Expression<Func<object>> dlgt = () => new SomeClass().DoSomething();
            ReflectionHelper.GetMethod<Func<object>>(dlgt).Name.ShouldEqual(expected);

            Expression<Func<int,int,object>> twoTypeParamDlgt = (n1,n2) => new SomeClass().DoSomething(n1,n2);
            ReflectionHelper.GetMethod(twoTypeParamDlgt).Name.ShouldEqual(expected);
        }

        [Test]
        public void can_get_property()
        {
            Expression<Func<Target, ChildTarget>> expression = t => t.Child;
            const string expected = "Child";
            ReflectionHelper.GetProperty(expression).Name.ShouldEqual(expected);

            LambdaExpression lambdaExpression = expression;
            ReflectionHelper.GetProperty(lambdaExpression).Name.ShouldEqual(expected);
        }

        [Test]
        public void GetProperty_should_throw_if_not_property_expression()
        {
            Expression<Func<SomeClass, object>> expression = c => c.DoSomething();
            typeof (ArgumentException).ShouldBeThrownBy(() => ReflectionHelper.GetProperty(expression)).
                Message.ShouldContain("Not a member access");
        }

        [Test]
        public void should_tell_if_is_member_expression()
        {
            Expression<Func<Target, ChildTarget>> expression = t => t.Child;
            Expression<Func<Target, object>> memberExpression = t => t.Child;
            ReflectionHelper.IsMemberExpression(expression).ShouldBeTrue();
            ReflectionHelper.IsMemberExpression(memberExpression).ShouldBeTrue();
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

    [TestFixture]
    public class SinglePropertyTester
    {
        public class Target
        {
            public string Name { get; set; }
            public ChildTarget Child { get; set; }
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

        [Test]
        public void DeclaringType_of_a_single_property_is_type_of_the_object_containing_the_property()
        {
            Accessor accessor = ReflectionHelper.GetAccessor<Target>(x => x.Child);
            accessor.ShouldBeOfType<SingleProperty>().DeclaringType.ShouldEqual(typeof(Target));
        }

        [Test]
        public void equals_for_a_single_property()
        {
            SingleProperty prop1 = SingleProperty.Build<Target>(x => x.Name);
            SingleProperty prop2 = SingleProperty.Build<Target>(x => x.Name);
            SingleProperty prop3 = SingleProperty.Build<Target>(x => x.Child);

            prop1.ShouldEqual(prop2);
            prop1.ShouldNotEqual(prop3);
            prop1.Equals(null).ShouldBeFalse();
            prop1.Equals(prop1).ShouldBeTrue();
            prop1.ShouldEqual(prop1);
            prop1.Equals((object)null).ShouldBeFalse();
            prop1.Equals(42).ShouldBeFalse();
        }

        [Test]
        public void singleProperty_can_get_child_accessor()
        {
            Accessor expected = ReflectionHelper.GetAccessor<ChildTarget>(c => c.GrandChild.Name);
            SingleProperty.Build<Target>(t => t.Child.GrandChild).
                GetChildAccessor<GrandChildTarget>(t => t.Name).ShouldEqual(expected);
        }

        [Test]
        public void singleProperty_property_names_contains_single_value()
        {
            SingleProperty.Build<Target>(t => t.Child.GrandChild.Name).PropertyNames.
                ShouldHaveCount(1).ShouldContain("Name");
        }

        [Test]
        public void build_single_property()
        {
            SingleProperty prop1 = SingleProperty.Build<Target>("Child");
            SingleProperty prop2 = SingleProperty.Build<Target>(x => x.Child);
            prop1.ShouldEqual(prop2);
            prop1.Name.ShouldEqual("Child");
            prop1.OwnerType.ShouldEqual(typeof(Target));
        }

        [Test]
        public void GetFieldNameFromSingleProperty()
        {
            SingleProperty property = SingleProperty.Build<Target>(x => x.Name);
            ((object)property.FieldName).ShouldEqual("Name");
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
    }

    [TestFixture]
    public class PropertyChainTester
    {
        [SetUp]
        public void SetUp()
        {
            PropertyInfo top = ReflectionHelper.GetProperty<Target>(x => x.Child);
            PropertyInfo second = ReflectionHelper.GetProperty<ChildTarget>(x => x.GrandChild);
            PropertyInfo birthday = ReflectionHelper.GetProperty<GrandChildTarget>(x => x.BirthDay);

            _chain = new PropertyChain(new[]
            {
                new PropertyValueGetter(top),
                new PropertyValueGetter(second),
                new PropertyValueGetter(birthday),

            });
        }

        private PropertyChain _chain;

        public class Target
        {
            public string Name { get; set; }
            public ChildTarget Child { get; set; }
        }

        public class ChildTarget
        {
            public ChildTarget()
            {
                GrandChildren = new List<GrandChildTarget>();
            }

            public int Age { get; set; }
            public GrandChildTarget GrandChild { get; set; }
            public GrandChildTarget SecondGrandChild { get; set; }
            public IList<GrandChildTarget> GrandChildren { get; set; }
        }

        public class GrandChildTarget
        {
            public DateTime BirthDay { get; set; }
            public string Name { get; set; }
        }

        [Test]
        public void property_chain_equals()
        {
            Accessor chain1 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay);
            Accessor chain2 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay);
            Accessor chain3 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild);
            Accessor chain4 = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.Name);
            Accessor chain5 = ReflectionHelper.GetAccessor<Target>(t => t.Child.SecondGrandChild.BirthDay);
            
            chain1.ShouldEqual(chain2);
            chain1.ShouldNotEqual(chain3);
            chain1.ShouldNotEqual(chain4);
            chain1.ShouldNotEqual(chain5);
            chain1.Equals(null).ShouldBeFalse();
            ((PropertyChain)chain1).Equals((PropertyChain)null).ShouldBeFalse();
            ((PropertyChain)chain1).Equals((PropertyChain)chain1).ShouldBeTrue();
            ((PropertyChain)chain1).Equals(1).ShouldBeFalse();
            chain1.ShouldEqual(chain1);

        }

        [Test]
        public void propertyChain_hashcode()
        {
            var chain = (PropertyChain)ReflectionHelper.GetAccessor<Target>(t => t.Child.Age);
            chain.GetHashCode().ShouldBeGreaterThan(0);
        }

        [Test]
        public void propertyChain_can_get_inner_property()
        {
            var chain = (PropertyChain)ReflectionHelper.GetAccessor<Target>(t => t.Child.Age);
            chain.InnerProperty.ShouldBeTheSameAs(typeof(ChildTarget).GetProperty("Age"));
        }

        [Test]
        public void propertyChain_can_get_child_accessor()
        {
            Accessor expected = ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.Name);
            ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild).
                GetChildAccessor<GrandChildTarget>(t => t.Name).ShouldEqual(expected);
        }

        [Test]
        public void propertyChain_can_get_the_name()
        {
            ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.BirthDay).Name.ShouldEqual(
                "ChildGrandChildBirthDay");
        }

        [Test]
        public void propertyChain_can_get_owner_type()
        {
            ReflectionHelper.GetAccessor<Target>(t => t.Child.Age).OwnerType.ShouldEqual(typeof(ChildTarget));
        }

        [Test]
        public void propertyChain_can_get_properties_names()
        {
            string[] names = ReflectionHelper.GetAccessor<Target>(t => t.Child.Age).PropertyNames;
            names.ShouldHaveCount(2);
            names.ShouldHave(n => n == "Child");
            names.ShouldHave(n => n == "Age");
        }

        [Test]
        public void propertyChain_toString_returns_graph()
        {
            ReflectionHelper.GetAccessor<Target>(t => t.Child.GrandChild.Name.Length).
                ToString().ShouldEqual("FubuCore.Testing.Reflection.PropertyChainTester+TargetChild.GrandChild.Name");
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
            _chain.PropertyType.ShouldEqual(typeof(DateTime));
        }

        [Test]
        public void CollectionIndexingPropertyAccessWorks()
        {
            Expression<Func<Target, object>> expression = x => x.Child.GrandChildren[0].Name;
            var accessor = expression.ToAccessor();

            var target = new Target
            {
                Child = new ChildTarget
                {
                    GrandChildren = { new GrandChildTarget { Name = "Bob" } }
                }
            };

            accessor.GetValue(target).ShouldEqual("Bob");
            accessor.Name.ShouldEqual("ChildGrandChildren[0]Name");
        }
    }
}