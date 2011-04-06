using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuTestingSupport;
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

}