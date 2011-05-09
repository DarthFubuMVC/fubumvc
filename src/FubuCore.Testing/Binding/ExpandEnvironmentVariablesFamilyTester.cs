using System;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
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
            var context = new InMemoryStructureMapBindingContext();
            context[expandProp.Name] = "%SystemRoot%\\foo";

            bool wasCalled = false;
            ValueConverter converter = _family.Build(null, expandProp);
            context.ForProperty(expandProp, x =>
            {
                wasCalled = true;

                converter.Convert(context).ShouldEqual(expandedVariable + @"\foo");
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