using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class WriteStringTester
    {
        [Test]
        public void to_object_def_builds_a_string_writer()
        {
            new WriteString().As<IContainerModel>()
                .ToObjectDef(DiagnosticLevel.None)
                .FindDependencyDefinitionFor<IMediaWriter<string>>()
                .Type
                .ShouldEqual(typeof (StringWriter));
        }

        [Test]
        public void mime_type_is_plain_text()
        {
            new WriteString().Mimetypes.Single()
                .ShouldEqual(MimeType.Text.Value);
        }
    }
}