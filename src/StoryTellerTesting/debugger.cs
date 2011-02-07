using System;
using System.Diagnostics;
using System.Linq.Expressions;
using NUnit.Framework;
using FubuCore.Reflection;
using FubuMVC.Tests;

namespace IntegrationTesting
{
    [TestFixture]
    public class debugger
    {
        [Test]
        public void try_to_set_up_the_environment()
        {
            var system = new FubuSystem();
            system.SetupEnvironment();
        }

        [Test]
        public void TESTNAME()
        {
            Expression<Func<One, object>> expression = x => x.Two.Three.Name;

            var parameter = Expression.Parameter(typeof(One), "x");
            var prop1 = ReflectionHelper.GetProperty<One>(x => x.Two);
            var prop2 = ReflectionHelper.GetProperty<Two>(x => x.Three);
            var prop3 = ReflectionHelper.GetProperty<Three>(x => x.Name);

            var body1 = Expression.Property(parameter, prop1);
            var body2 = Expression.Property(body1, prop2);
            var body3 = Expression.Property(body2, prop3);

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(One), typeof(object));
            var lamda = Expression.Lambda(delegateType, body3, parameter);

            Debug.WriteLine(expression.ToString());
        }

        public class One
        {
            public Two Two { get; set; }
        }

        public class Two
        {
            public Three Three { get; set; }
        }

        public class Three
        {
            public string Name { get; set; }
        }
    }
}