using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Resources.Conneg
{
    
    public class ModelBindingReaderTester : InteractionContext<ModelBindingReader<Address>>
    {
        [Fact]
        public void mime_types()
        {
            ClassUnderTest.Mimetypes.ShouldHaveTheSameElementsAs(MimeType.HttpFormMimetype, MimeType.MultipartMimetype);
        }

        [Fact]
        public void read_just_forces_ifuburequest_to_read()
        {
            var address = new Address();

            var context = new MockedFubuRequestContext(Services.Container);
            Services.Inject<IFubuRequestContext>(context);


            MockFor<IFubuRequest>().Stub(x => x.Get<Address>())
                .Return(address);

            ClassUnderTest.Read("anything", context).ShouldBeTheSameAs(address);

            MockFor<IFubuRequest>().Clear(typeof(Address));
        }
    }
}