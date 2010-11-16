using FubuCore;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Tags;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class ServiceLocatorExtensionsTester
    {
        [Test]
        public void get_tags_for_a_model_type()
        {
            var theModel = new Model1();

            var locator = MockRepository.GenerateMock<IServiceLocator>();
            var types = MockRepository.GenerateMock<ITypeResolver>();

            locator.Stub(x => x.GetInstance<ITypeResolver>()).Return(types);
            types.Stub(x => x.ResolveType(theModel)).Return(typeof (Model2));

            var tags = MockRepository.GenerateMock<ITagGenerator<Model2>>();
            locator.Stub(x => x.GetInstance(typeof(ITagGenerator<Model2>))).Return(tags);

            locator.TagsFor(theModel).ShouldBeTheSameAs(tags);
        }

        public class Model1{}
        public class Model2 : Model1{}
    }
}