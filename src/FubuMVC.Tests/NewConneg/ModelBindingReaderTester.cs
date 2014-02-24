using FubuMVC.Core;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class ModelBindingReaderTester : InteractionContext<ModelBindingReader<Address>>
    {
        [Test]
        public void mime_types()
        {
            ClassUnderTest.Mimetypes.ShouldHaveTheSameElementsAs(MimeType.HttpFormMimetype, MimeType.MultipartMimetype);
        }

        [Test]
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