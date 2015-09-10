using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Resources.Conneg
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