using FubuMVC.Core.Resources.Conneg.New;
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
            MockFor<IFubuRequest>().Stub(x => x.Get<Address>())
                .Return(address);

            ClassUnderTest.Read("anything").ShouldBeTheSameAs(address);

            MockFor<IFubuRequest>().Clear(typeof(Address));
        }
    }
}