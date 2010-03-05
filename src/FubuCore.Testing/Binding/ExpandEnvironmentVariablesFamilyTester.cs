using System;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Models;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class ExpandEnvironmentVariablesFamilyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _family = new ExpandEnvironmentVariablesFamily();
            expandProp = ReflectionHelper.GetProperty<TestSettings>(t => t.DefaultPath);
            noExpandProp = ReflectionHelper.GetProperty<TestSettings>(t => t.DoNotExpand);
        }

        #endregion

        private ExpandEnvironmentVariablesFamily _family;
        private PropertyInfo noExpandProp;
        private PropertyInfo expandProp;

        public class TestSettings
        {
            [ExpandEnvironmentVariables]
            public string DefaultPath { get; set; }

            public string DoNotExpand { get; set; }
        }

        [Test]
        public void expand_environment_variables_for_settings_marked_for_expansion()
        {
            string expandedVariable = Environment.GetEnvironmentVariable("SystemRoot");
            var context = new InMemoryBindingContext();
            context[expandProp.Name] = "%SystemRoot%\\foo";

            bool wasCalled = false;
            ValueConverter converter = _family.Build(null, expandProp);
            context.ForProperty(expandProp, () =>
            {
                wasCalled = true;

                converter(context).ShouldEqual(expandedVariable + @"\foo");
            });

            wasCalled.ShouldBeTrue();
        }

        [Test]
        public void should_match_properties_with_attribute()
        {
            _family.Matches(expandProp).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_properties_without_attribute()
        {
            _family.Matches(noExpandProp).ShouldBeFalse();
        }
    }
}