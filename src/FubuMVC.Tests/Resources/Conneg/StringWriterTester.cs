using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Resources.Conneg
{
    
    public class StringWriterTester : InteractionContext<StringWriter>
    {
        [Fact]
        public void mime_type_is_only_text()
        {
            ClassUnderTest.Mimetypes
                .ShouldHaveTheSameElementsAs(MimeType.Text.Value, MimeType.Html.Value);
        }

        [Fact]
        public void write()
        {
            var context = new MockedFubuRequestContext();
            ClassUnderTest.Write(MimeType.Text.Value, context, "some text");
            context.Writer.AssertWasCalled(x => x.Write(MimeType.Text.Value, "some text"));
        }
    }

    
}