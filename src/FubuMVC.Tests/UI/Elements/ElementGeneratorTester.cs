using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Elements;
using FubuTestingSupport;
using HtmlTags;
using HtmlTags.Conventions;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Elements
{
    [TestFixture]
    public class ElementGeneratorTester : InteractionContext<ElementGenerator<Address>>
    {
        private Address theAddress;

        protected override void beforeEach()
        {
            theAddress = new Address{
                Address1 = "22 Cherry Lane"
            };

            

            MockFor<IFubuRequest>().Stub(x => x.Get<Address>()).Return(theAddress);
            Services.Inject<ITagGenerator<ElementRequest>>(new AddressTagGenerator());

            MockFor<ITagGeneratorFactory>().Stub(x => x.GeneratorFor<ElementRequest>()).Return(new AddressTagGenerator());
        }

        [Test]
        public void happily_finds_the_model_from_fubu_request_if_it_is_missing()
        {
            ClassUnderTest.Model.ShouldBeTheSameAs(theAddress);
        }

        [Test]
        public void uses_the_model_from_FubuRequest_if_none_is_set()
        {
            var tag = ClassUnderTest.LabelFor(x => x.Address1);
            tag.Text().ShouldEqual(theAddress.Address1);
        }

        [Test]
        public void can_override_the_model()
        {
            var address2 = new Address(){
                Address1 = "RR 2"
            };

            ClassUnderTest.Model = address2;
            ClassUnderTest.Model.ShouldBeTheSameAs(address2);

            var tag = ClassUnderTest.LabelFor(x => x.Address1);
            tag.Text().ShouldEqual(address2.Address1);
        }

        [Test]
        public void label_for_with_no_profile()
        {
            var tag = ClassUnderTest.LabelFor(x => x.Address1);
            tag.Attr("category").ShouldEqual(ElementConstants.Label);
            tag.HasAttr("profile").ShouldBeFalse();
        }

        [Test]
        public void label_for_with_profile()
        {
            var tag = ClassUnderTest.LabelFor(x => x.Address1, "NewPage");
            tag.Attr("category").ShouldEqual(ElementConstants.Label);  
            tag.Attr("profile").ShouldEqual("NewPage");  
        }

        [Test]
        public void input_for_with_no_profile()
        {
            var tag = ClassUnderTest.InputFor(x => x.Address1);
            tag.Attr("category").ShouldEqual(ElementConstants.Editor);
            tag.HasAttr("profile").ShouldBeFalse();
        }

        [Test]
        public void input_for_with_profile()
        {
            var tag = ClassUnderTest.InputFor(x => x.Address1, "NewPage");
            tag.Attr("category").ShouldEqual(ElementConstants.Editor);
            tag.Attr("profile").ShouldEqual("NewPage");
        }

        [Test]
        public void display_for_with_no_profile()
        {
            var tag = ClassUnderTest.DisplayFor(x => x.Address1);
            tag.Attr("category").ShouldEqual(ElementConstants.Display);
            tag.HasAttr("profile").ShouldBeFalse();
        }

        [Test]
        public void display_for_with_profile()
        {
            var tag = ClassUnderTest.DisplayFor(x => x.Address1, "NewPage");
            tag.Attr("category").ShouldEqual(ElementConstants.Display);
            tag.Attr("profile").ShouldEqual("NewPage");
        }
    }

    public class AddressTagGenerator : ITagGenerator<ElementRequest>
    {
        public HtmlTag Build(ElementRequest request, string category, string profile)
        {
            var tag = new HtmlTag("div").Text(request.RawValue.ToString());
            if (category.IsNotEmpty())
            {
                tag.Attr("category", category);
            }

            if (profile.IsNotEmpty())
            {
                tag.Attr("profile", profile);
            }

            return tag;
        }

        public string ActiveProfile
        {
            get; set;
        }
    }
}