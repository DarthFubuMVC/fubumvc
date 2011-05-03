using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FubuTestingSupport;
using NUnit.Framework;
using System;

namespace FubuCore.Testing
{
    [TestFixture]
    public class StringExtensionsTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void combine_to_path_when_rooted()
        {
			var rooted = Path.Combine(Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory), "here");
            rooted.CombineToPath("there").ShouldEqual(rooted);
        }

        [Test]
        public void combine_to_path_when_not_rooted()
        {
            "here".CombineToPath("there").ShouldEqual(Path.Combine("there", "here"));
        }

        [Test]
        public void is_empty()
        {
            string.Empty.IsEmpty().ShouldBeTrue();

            string nullString = null;
            nullString.IsEmpty().ShouldBeTrue();
        
            " ".IsEmpty().ShouldBeFalse();
            "something".IsEmpty().ShouldBeFalse();
        }

        [Test]
        public void is_not_empty()
        {
            string.Empty.IsNotEmpty().ShouldBeFalse();

            string nullString = null;
            nullString.IsNotEmpty().ShouldBeFalse();

            " ".IsNotEmpty().ShouldBeTrue();
            "something".IsNotEmpty().ShouldBeTrue();
        }


        [Test]
        public void converting_plain_text_line_returns_to_line_breaks()
        {
            const string plainText = "before\nmiddle\r\nafter";

            var textWithBreaks = plainText.ConvertCRLFToBreaks();

            textWithBreaks.ShouldEqual(@"before<br/>middle<br/>after");
        }

        [Test]
        public void numbers_with_commas_and_periods_should_be_valid()
        {
            var numbers = new[]
                              {
                                  "1,000",
                                  "100.1",
                                  "1000.1",
                                  "1,000.1",
                                  "10,000.1",
                                  "100,000.1",
                              };

            numbers.Each(x => x.IsValidNumber(CultureInfo.CreateSpecificCulture("en-us")).ShouldBeTrue());
          
        }

        [Test]
        public void numbers_with_commas_and_periods_should_be_valid_in_european_culture()
        {
            var numbers = new[]
                              {
                                  "1.000",
                                  "100,1",
                                  "1000,1",
                                  "1.000,1",
                                  "10.000,1",
                                  "100.000,1",
                              };

            numbers.Each(x => x.IsValidNumber(CultureInfo.CreateSpecificCulture("de-DE")).ShouldBeTrue());
          
        }

        [Test]
        public void numbers_should_be_invalid()
        {
            var numbers = new[]
                              {
                                  "1,00",
                                  "100,1",
                                  "100,1.01",
                                  "A,Jun.K",
                              };

            numbers.Each(x => x.IsValidNumber(CultureInfo.CreateSpecificCulture("en-us")).ShouldBeFalse());
        }

        [Test]
        public void to_bool()
        {
            "true".ToBool().ShouldBeTrue();
            "True".ToBool().ShouldBeTrue();
        
            "false".ToBool().ShouldBeFalse();
            "False".ToBool().ShouldBeFalse();
        
            "".ToBool().ShouldBeFalse();

            string nullString = null;
            nullString.ToBool().ShouldBeFalse();
        }

        [Test]
        public void to_format()
        {
            "My name is {0} and I was born in {1}, {2}".ToFormat("Jeremy", "Carthage", "Missouri")
                .ShouldEqual("My name is Jeremy and I was born in Carthage, Missouri");
        }
    }
}