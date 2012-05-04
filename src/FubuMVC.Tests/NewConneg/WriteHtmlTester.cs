using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using HtmlTags;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class WriteHtmlTester
    {
        [Test]
        public void build_the_object_def()
        {
            var node = new WriteHtml(typeof (HtmlTag));
            node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None)
                .FindDependencyDefinitionFor<IMediaWriter<HtmlTag>>()
                .Type.ShouldEqual(typeof (HtmlStringWriter<HtmlTag>));
        }

        [Test]
        public void mime_type_is_only_html()
        {
            new WriteHtml(typeof (HtmlTag))
                .Mimetypes.Single()
                .ShouldEqual(MimeType.Html.Value);
        }
    }
}