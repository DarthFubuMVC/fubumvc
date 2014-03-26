using System.Collections.Generic;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.UI;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.UI
{
    public class Child
    {
        public string Name { get; set; }
    }

    public class Parent
    {
        public IList<Child> Children { get; set; }
    }

    public class IndexerOwnerEndpoint
    {
        private readonly FubuHtmlDocument<Parent> _document;

        public IndexerOwnerEndpoint(FubuHtmlDocument<Parent> document)
        {
            _document = document;
        }

        public HtmlDocument get_template_for_indexer()
        {
            _document.Add(_document.InputFor(new Parent(), x => x.Children[-1]));

            return _document;
        }
    }

    [TestFixture]
    public class indexer_owner_type_bug_tester
    {
        [Test]
        public void should_build_the_template_just_fine()
        {
            using (var server = FubuApplication
                .DefaultPolicies()
                .StructureMap(new Container())
                .RunEmbeddedWithAutoPort())
            {
                var response = server.Endpoints
                    .Get<IndexerOwnerEndpoint>(x => x.get_template_for_indexer());

                response
                      .StatusCode.ShouldEqual(HttpStatusCode.OK);

                response.ToString()
                    .ShouldContain("<input type=\"text\" value=\"\" name=\"Children[-1]\" />");
            }
        }
    }
}