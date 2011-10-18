using FubuCore.Binding;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Tests.UI;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Resources.Media
{
    [TestFixture]
    public class ModelBindingMediaReaderTester : InteractionContext<ModelBindingMediaReader<Address>>
    {
        [Test]
        public void only_mime_type_is_the_default_form_post()
        {
            ClassUnderTest.Mimetypes.ShouldHaveTheSameElementsAs("application/x-www-form-urlencoded", "multipart/form-data");
        }

        [Test]
        public void read_just_delegates_to_object_resolver()
        {
            var theAddress = new Address();
            MockFor<IObjectResolver>().Stub(x => x.BindModel(typeof (Address), MockFor<IBindingContext>()))
                .Return(new BindResult() { Value = theAddress });

            ClassUnderTest.Read("something").ShouldBeTheSameAs(theAddress);

        }
    }
}