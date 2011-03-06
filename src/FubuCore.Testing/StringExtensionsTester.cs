using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class StringExtensionsTester
    {
        static readonly char Sep = Path.DirectorySeparatorChar;

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void combining_path_with_empty_yields_path()
        {
            var path1 = AppDomain.CurrentDomain.BaseDirectory;

            path1.CombinePath(string.Empty).ShouldEqual(path1);
            path1.CombinePath(string.Empty, string.Empty).ShouldEqual(path1);
            path1.CombinePath(string.Empty, string.Empty, string.Empty).ShouldEqual(path1);
        }

        [Test]
        public void combining_path1_with_path2_yields_combined()
        {
            var path1 = AppDomain.CurrentDomain.BaseDirectory;

            path1.CombinePath("p2").ShouldEqual(path1 + Sep + "p2");
        }

        [Test]
        public void combining_path1_with_path2_and_additionalpaths_yields_combined()
        {
            var path1 = AppDomain.CurrentDomain.BaseDirectory;
            const string path2 = "p2";
            var additionals = new[] {"p3", "p4"};
            
            var expected = 
                path1 + Sep + 
                path2 + Sep + 
                additionals[0] + Sep + 
                additionals[1];

            path1.CombinePath(path2, additionals).ShouldEqual(expected);
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