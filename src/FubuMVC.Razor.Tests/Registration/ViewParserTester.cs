using System.Collections.Generic;
using System.Linq;
using System.Web.Razor.Parser.SyntaxTree;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.Registration;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Razor.Tests.Registration
{
    [TestFixture]
    public class ViewParserTester
    {
        private IEnumerable<Span> _results;

        [SetUp]
        public void Setup()
        {
            var parser = new ViewParser();
            _results = parser.Parse("Templates/ToParse.cshtml");
        }

        [Test]
        public void can_be_empty_master()
        {
            var parser = new ViewParser();
            var results = parser.Parse("Templates/HasEmptyMaster.cshtml");
            results.Master().ShouldEqual(string.Empty);
        }

        [Test]
        public void will_parse_master()
        {
            _results.Master().ShouldEqual("MyLayout");
        }

        [Test]
        public void will_parse_namespaces()
        {
            _results.Namespaces().First().ShouldEqual("MyNamespace");
        }

        [Test]
        public void will_parse_viewmodel()
        {
            _results.ViewModel().ShouldEqual("MyModel");
        }
    }
}