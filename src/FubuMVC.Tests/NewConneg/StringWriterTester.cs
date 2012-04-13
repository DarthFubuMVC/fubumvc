using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class StringWriterTester : InteractionContext<StringWriter>
    {
        [Test]
        public void mime_type_is_only_text()
        {
            ClassUnderTest.Mimetypes.Single()
                .ShouldEqual(MimeType.Text.Value);
        }

        [Test]
        public void write()
        {
            ClassUnderTest.Write(MimeType.Text.Value, "some text");
            MockFor<IOutputWriter>().AssertWasCalled(x => x.Write(MimeType.Text.Value, "some text"));
        }
    }
}