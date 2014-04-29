using System.Text;
using FubuMVC.Core.UI.Extensions;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Extensions
{
    [TestFixture]
    public class ContentExtensionGraphTester
    {
        private ContentExtensionGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            theGraph = new ContentExtensionGraph();
        }

        [Test]
        public void applying_the_extension_builds_up_html_string()
        {
            var e1 = MockRepository.GenerateStub<IContentExtension<ExtensionShelfModel>>();
            var e2 = MockRepository.GenerateStub<IContentExtension<ExtensionShelfModel>>();

            var line1 = "Test1";
            var line2 = "Test2";

            e1.Stub(x => x.GetExtensions(null)).IgnoreArguments().Return(new object[] {line1});
            e2.Stub(x => x.GetExtensions(null)).IgnoreArguments().Return(new object[] { line2 });

            var expected = new StringBuilder().AppendLine(line1).AppendLine(line2).ToString();

            theGraph.Register(e1);
            theGraph.Register(e2);

            theGraph.ApplyExtensions<ExtensionShelfModel>(null).ToString().ShouldEqual(expected);
        }

        [Test]
        public void applying_the_extension_builds_up_html_string_for_the_tag()
        {
            var e1 = MockRepository.GenerateStub<IContentExtension<ExtensionShelfModel>>();
            var e2 = MockRepository.GenerateStub<IContentExtension<ExtensionShelfModel>>();

            var line1 = "Test1";
            var line2 = "Test2";

            e1.Stub(x => x.GetExtensions(null)).IgnoreArguments().Return(new object[] { line1 });
            e2.Stub(x => x.GetExtensions(null)).IgnoreArguments().Return(new object[] { line2 });

            var expected = new StringBuilder().AppendLine(line2).ToString();

            var tag = "tag";
            theGraph.Register(e1);
            theGraph.Register(tag, e2);

            theGraph.ApplyExtensions<ExtensionShelfModel>(null, tag).ToString().ShouldEqual(expected);
        }
    }
}