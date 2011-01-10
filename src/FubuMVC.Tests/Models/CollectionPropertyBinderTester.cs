using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.StructureMap;
using FubuMVC.Tests.UI;
using NUnit.Framework;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class CollectionPropertyBinderTester : PropertyBinderTester
    {
        [SetUp]
        public void Setup()
        {
            context = new InMemoryBindingContext();
            var container = StructureMapContainerFacility.GetBasicFubuContainer();
            var objectResolver = container.GetInstance<ObjectResolver>();
            context.Container.Configure(x => x.For<IObjectResolver>().Use(objectResolver));
            propertyBinder = new CollectionPropertyBinder(new DefaultCollectionTypeProvider());
        }

        [Test]
        public void matches_collection_properties()
        {
            shouldMatch(x => x.Localities);
        }

        [Test]
        public void doesnt_match_any_other_properties()
        {
            shouldNotMatch(x => x.Address);
            shouldNotMatch(x => x.Address.Address1);
            shouldNotMatch(x => x.Address.Order);
            shouldNotMatch(x => x.Address.IsActive);
            shouldNotMatch(x => x.Address.DateEntered);
            shouldNotMatch(x => x.Address.Color);
            shouldNotMatch(x => x.Address.Guid);
        }

        [Test]
        public void set_a_property_correctly_against_a_binding_context()
        {
            var model = new AddressViewModel();
            context.WithData("Localities[0]ZipCode", "84115");
            context.StartObject(model);

            var property = ReflectionHelper.GetProperty<AddressViewModel>(x => x.Localities);
            propertyBinder.Bind(property, context);

            model.Localities[0].ZipCode.ShouldEqual("84115");
        }
    }
}