using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using FubuMVC.Core.Models;
using FubuMVC.Core.Util;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class ResolveConnectionStringFamilyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            family = new ResolveConnectionStringFamily();

            expandProp = ReflectionHelper.GetProperty<TestSettings>(t => t.DefaultPath);
            noExpandProp = ReflectionHelper.GetProperty<TestSettings>(t => t.DoNotExpand);

            configuredConnectionStringSetting = new ConnectionStringSettings(connectionStringKey, actualConnectionString);
            var connectionStrings = new Dictionary<string, ConnectionStringSettings>
            {
                {connectionStringKey, configuredConnectionStringSetting}
            };
            ResolveConnectionStringFamily.GetConnectionStringSettings =
                key => connectionStrings.ContainsKey(key) ? connectionStrings[key] : null;
        }

        #endregion

        private ResolveConnectionStringFamily family;
        private PropertyInfo noExpandProp;
        private PropertyInfo expandProp;
        private const string connectionStringKey = "DBKey";
        private const string actualConnectionString = "data source=test;user id=person;password=sesame";
        private ConnectionStringSettings configuredConnectionStringSetting;

        public class TestSettings
        {
            [ConnectionString]
            public string DefaultPath { get; set; }

            public string DoNotExpand { get; set; }
        }

        [Test]
        public void return_the_original_value_if_the_connection_string_doesnt_exist()
        {
            var value = new InMemoryBindingContext().WithPropertyValue("foo");

            object result = family.Build(null, expandProp)(value);
            result.ShouldEqual("foo");
        }

        [Test]
        public void return_the_value_from_the_connectionStrings_section()
        {
            var context = new InMemoryBindingContext().WithPropertyValue(connectionStringKey);

            object result = family.Build(null, expandProp)(context);
            result.ShouldEqual(actualConnectionString);
        }

        [Test]
        public void should_match_properties_with_attribute()
        {
            family.Matches(expandProp).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_properties_without_attribute()
        {
            family.Matches(noExpandProp).ShouldBeFalse();
        }
    }
}