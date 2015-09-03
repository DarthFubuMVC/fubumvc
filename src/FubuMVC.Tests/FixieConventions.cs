using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fixie;
using FubuCore;
using FubuCore.Reflection;
using NUnit.Framework;

namespace NUnit.Framework
{
    public class TestFixtureAttribute : Attribute { }
    public class TestAttribute : Attribute { }

    public class ExplicitAttribute : Attribute
    {
        private readonly string _description;

        public ExplicitAttribute()
        {
        }

        public ExplicitAttribute(string description)
        {
            _description = description;
        }

        public string description
        {
            get { return _description; }
        }
    }
    public class TestFixtureSetUpAttribute : Attribute { }
    public class SetUpAttribute : Attribute { }
    public class TearDownAttribute : Attribute { }
    public class TestFixtureTearDownAttribute : Attribute { }

    public static class Assert
    {
        public static void Fail(string message, params object[] parameters)
        {
            throw new Exception(message.ToFormat(parameters));
        }


    }
}

namespace FubuMVC.Tests
{

    public class CustomConvention : Convention
    {
        public CustomConvention()
        {
            Classes
                .HasOrInherits<TestFixtureAttribute>().Where(type => !type.HasAttribute<ExplicitAttribute>());

            Methods
                .HasOrInherits<TestAttribute>().Where(m => !m.HasAttribute<ExplicitAttribute>());

            ClassExecution
                    .CreateInstancePerClass()
                    .SortCases((caseA, caseB) => String.Compare(caseA.Name, caseB.Name, StringComparison.Ordinal));

            FixtureExecution
                .Wrap<FixtureSetUpTearDown>();

            CaseExecution
                .Wrap<SetUpTearDown>();
        }
    }


    class SetUpTearDown : CaseBehavior
    {
        public void Execute(Case @case, Action next)
        {
            @case.Class.InvokeAll<SetUpAttribute>(@case.Fixture.Instance);
            next();
            @case.Class.InvokeAll<TearDownAttribute>(@case.Fixture.Instance);
        }
    }

    class FixtureSetUpTearDown : FixtureBehavior
    {

        public void Execute(Fixie.Fixture fixture, Action next)
        {
            fixture.Class.Type.InvokeAll<TestFixtureSetUpAttribute>(fixture.Instance);
            next();
            fixture.Class.Type.InvokeAll<TestFixtureTearDownAttribute>(fixture.Instance);
        }
    }

    public static class BehaviorBuilderExtensions
    {
        public static void InvokeAll<TAttribute>(this Type type, object instance)
            where TAttribute : Attribute
        {
            foreach (var method in Has<TAttribute>(type))
            {
                try
                {
                    method.Invoke(instance, null);
                }
                catch (TargetInvocationException exception)
                {
                    throw new PreservedException(exception.InnerException);
                }
            }
        }

        static IEnumerable<MethodInfo> Has<TAttribute>(Type type) where TAttribute : Attribute
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.HasOrInherits<TAttribute>());
        }
    }
}