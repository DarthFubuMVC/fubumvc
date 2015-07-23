using System;
using System.Reflection;
using FubuMVC.Core.Registration.Routes;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class FuncBuilderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            type = typeof (PropertyTarget);
        }

        #endregion

        private Type type;

        public class PropertyTarget
        {
            public string Name { get; set; }
            public string LastCallToGo { get; set; }

            public void OneInZeroOut(string name)
            {
                LastCallToGo = name;
            }

            public string ZeroInOneOut()
            {
                return "Jeremy";
            }

            public string OneInOneOut(int name)
            {
                return name.ToString();
            }
        }


        [Test]
        public void one_in_one_out()
        {
            MethodInfo method = type.GetMethod("OneInOneOut");
            var func = FuncBuilder.ToFunc(type, method).ShouldBeOfType<Func<PropertyTarget, int, string>>();
            func(new PropertyTarget(), 123).ShouldBe("123");
        }

        [Test]
        public void one_in_zero_out()
        {
            MethodInfo method = type.GetMethod("OneInZeroOut");
            var action = FuncBuilder.ToAction(type, method).ShouldBeOfType<Action<PropertyTarget, string>>();

            var target = new PropertyTarget();

            action(target, "last name");

            target.LastCallToGo.ShouldBe("last name");
        }

        [Test]
        public void zero_in_one_out()
        {
            MethodInfo method = type.GetMethod("ZeroInOneOut");
            var func = FuncBuilder.ToFunc(type, method).ShouldBeOfType<Func<PropertyTarget, string>>();

            var target = new PropertyTarget();

            func(target).ShouldBe(target.ZeroInOneOut());
        }
    }
}