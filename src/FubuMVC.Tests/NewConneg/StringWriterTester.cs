using FubuMVC.Core.Resources.Conneg;
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
            ClassUnderTest.Mimetypes
                .ShouldHaveTheSameElementsAs(MimeType.Text.Value, MimeType.Html.Value);
        }

        [Test]
        public void write()
        {
            var context = new MockedFubuRequestContext();
            ClassUnderTest.Write(MimeType.Text.Value, context, "some text");
            context.Writer.AssertWasCalled(x => x.Write(MimeType.Text.Value, "some text"));
        }
    }

    
}