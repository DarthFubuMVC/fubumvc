using System.Collections.Generic;
using FubuCore;
using FubuMVC.Spark.SparkModel.Parsing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Parsing
{
    public class SparkParserTester
    {
        
    }

    [TestFixture]
    public class SparkParserExtensionsTester
    {
        private readonly ISparkParser _parser;
        public SparkParserExtensionsTester()
        {
            var extractor = new ElementNodeExtractor();
            _parser = new SparkParser(extractor);
        }

        [Test]
        public void when_no_view_model_is_present_null_is_returned()
        {
            var content = 
                Templates.UseMaster.ToFormat("Fubu") + 
                Templates.UseNamespace.ToFormat("A.B.C") +
                Templates.Content.ToFormat("parsing");
            
            _parser.ParseViewModelTypeName(content).ShouldBeNull();
        }

        [Test]
        public void when_view_model_is_present_type_name_is_returned()
        {
            var content = 
                Templates.UseMaster.ToFormat("Spark") +
                Templates.ViewdataModel.ToFormat(GetType().FullName);

            _parser.ParseViewModelTypeName(content).ShouldEqual(GetType().FullName);
        }   
      
        [Test]
        public void when_multiple_viewdata_is_present_model_is_parsed()
        {
            const string viewdata = "<viewdata {0} />";
            const string arg = @"""{0}""=""{1}""";
            var arguments = new List<string>
            {
                arg.ToFormat("Caption", "string"),
                arg.ToFormat("Products", "System.Collections.Generic.IList[[MyApp.Models.Product]]")
            };

            var content = Templates.ViewdataModel.ToFormat(GetType().FullName) +
                          viewdata.ToFormat(arguments.Join(" ")) +
                          Templates.Content.ToFormat("FubuMVC.Spark");

            _parser.ParseViewModelTypeName(content).ShouldEqual(GetType().FullName);
        }

        [Test]
        public void when_master_is_present_it_is_parsed()
        {
            _parser.ParseMasterName(Templates.UseMaster.ToFormat("Universe"))
                .ShouldEqual("Universe");
        }

        [Test]
        public void when_master_is_present_and_empty_it_is_parsed_as_empty()
        {
            _parser.ParseMasterName(Templates.UseMaster.ToFormat(""))
                .ShouldBeEmpty();
        }

        [Test]
        public void when_master_is_absent_null_is_returned()
        {
            _parser.ParseMasterName(Templates.Content.ToFormat("FubuMVC"))
                .ShouldBeNull();
        }
    }
}