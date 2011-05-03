using System.Collections.Generic;
using Bottles.Deployment.Parsing;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace Bottles.Tests.Deployment.Parsing
{
    [TestFixture]
    public class SettingsParserTester
    {
        private SettingsParser theParser;
        private Dictionary<string, string> theDictionary;

        [SetUp]
        public void SetUp()
        {
            theDictionary = new Dictionary<string, string>();
            theParser = new SettingsParser("some description", theDictionary);
        }

        [Test]
        public void the_description_from_the_ctor_makes_it_to_the_settings_data()
        {
            theParser.Settings.Description.ShouldEqual("some description");
        }

        [Test]
        public void parsing_a_value_that_is_neither_bottle_not_property_will_throw_up()
        {
            Exception<SettingsParserException>.ShouldBeThrownBy(() =>
            {
                theParser.ParseText("junk");
            }).Message.Contains(SettingsParser.INVALID_SYNTAX);
        }

        [Test]
        public void parse_a_simple_name_value_pair()
        {
            theParser.ParseText("Class.Prop=1");
            theParser.Settings.Get("Class.Prop").ShouldEqual("1");
        }

        [Test]
        public void parse_a_value_with_subsitutions()
        {
            theDictionary.Add("dbname", "blue");
            theDictionary.Add("password", "superfly");

            theParser.ParseText("Class.ConnectionString=Name={dbname};password={password}");

            theParser.Settings.Get("Class.ConnectionString").ShouldEqual("Name=blue;password=superfly");
        }

        [Test]
        public void parse_a_missing_property_name()
        {
            Exception<SettingsParserException>.ShouldBeThrownBy(() =>
            {
                theParser.ParseText("=something");
            });
        }

        [Test]
        public void parse_a_missing_property_value_just_sets_it_to_empty_string()
        {
            theParser.ParseText("Class.Prop=");
            theParser.Settings.Get("Class.Prop").ShouldBeEmpty();
        }

        [Test]
        public void parse_a_simple_name_with_whitespace_1()
        {
            theParser.ParseText("Class.Prop=      1");
            theParser.Settings.Get("Class.Prop").ShouldEqual("1");
        }

        [Test]
        public void parse_a_simple_name_with_whitespace_2()
        {
            theParser.ParseText("Class.Prop   =      1");
            theParser.Settings.Get("Class.Prop").ShouldEqual("1");
        }

        [Test]
        public void parse_a_simple_name_with_whitespace_3()
        {
            theParser.ParseText("          Class.Prop   =      1");
            theParser.Settings.Get("Class.Prop").ShouldEqual("1");
        }

        [Test]
        public void parse_a_bottle_reference_with_no_relationship()
        {
            theParser.ParseText("bottle:webcore");

            var reference = theParser.References.Single();
            reference.Name.ShouldEqual("webcore");
        }

        [Test]
        public void parse_a_bottle_reference_with_a_relationship()
        {
            theParser.ParseText("bottle:webcore");

            var reference = theParser.References.Single();
            reference.Name.ShouldEqual("webcore");
        }


    }
}