using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Querying;
using FubuMVC.Tests.UI;
using HtmlTags;
using NUnit.Framework;
using System.Collections.Generic;
using TestPackage1;
using Container = StructureMap.Container;

namespace FubuMVC.Tests
{
    [TestFixture, Explicit]
    public class Debugging
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion


        [Test]
        public void try_dictionary()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("a", 1);
            dict.Add("b", "2");
            dict.Add("c", true);

            var s = JsonUtil.ToJson(dict);

            Debug.WriteLine(s);
        }

        [Test]
        public void try_out_the_test_package_1()
        {
            var container = new Container(x =>
            {
                x.Scan(o =>
                {
                    o.AssemblyContainingType<TestPackage1Registry>();
                    o.AddAllTypesOf<IFubuRegistryExtension>();
                });
            });

            Debug.WriteLine(container.WhatDoIHave());
        }

        [Test]
        public void try_to_execute_the_remote_behavior_graph()
        {
            var remote = new RemoteBehaviorGraph("http://localhost/fubu-testing");
            remote.All().AllEndpoints.Each(x => Debug.WriteLine(x.RoutePattern));
        }

        [Test]
        public void expression_playing()
        {
            Expression<Func<PropertyTarget, object>> expression = x => x.Name;


            Debug.WriteLine(expression);

            Type type = typeof (PropertyTarget);

            ParameterExpression parameter = Expression.Parameter(type, "x");
            MemberExpression member = Expression.MakeMemberAccess(parameter, type.GetProperty("Name"));

            Debug.WriteLine(member.ToString());

            Type delegateType = typeof (Func<PropertyTarget, object>);
            LambdaExpression newExp = Expression.Lambda(delegateType, member, parameter);

            Debug.WriteLine(newExp);

            var func = (Func<PropertyTarget, object>) newExp.Compile();

            func(new PropertyTarget
            {
                Name = "Jeremy"
            }).ShouldEqual("Jeremy");
        }

        [Test]
        public void get_type_converter_for_something_that_does_not_exist()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Address));
            Debug.WriteLine(converter);

            TypeDescriptor.GetConverter(typeof (bool)).CanConvertFrom(typeof (string));
            TypeDescriptor.GetConverter(typeof (Address)).CanConvertFrom(typeof (string));
        }

        [Test]
        public void method1_test()
        {
            Expression<Func<PropertyTarget, int, string>> expression = (x, input) => x.OneInOneOut(input);

            Debug.WriteLine(expression.ToString());

            Type type = typeof (PropertyTarget);
            MethodInfo method = type.GetMethod("OneInOneOut");

            ParameterExpression objectParameter = Expression.Parameter(type, "x");
            ParameterExpression inputParameter = Expression.Parameter(method.GetParameters()[0].ParameterType, "input");

            MethodCallExpression methodCall = Expression.Call(objectParameter, method, inputParameter);

            Type delegateType = typeof (Func<PropertyTarget, int, string>);
            LambdaExpression newExp = Expression.Lambda(delegateType, methodCall, objectParameter, inputParameter);
            Debug.WriteLine(newExp);

            var func = (Func<PropertyTarget, int, string>) newExp.Compile();

            func(new PropertyTarget(), 123).ShouldEqual("123");
        }
    }

    public class PropertyTarget
    {
        public string Name { get; set; }

        public void Go(string name)
        {
            Debug.WriteLine("Go " + name);
        }

        public string Get()
        {
            return "Jeremy";
        }

        public string OneInOneOut(int name)
        {
            return name.ToString();
        }
    }
}