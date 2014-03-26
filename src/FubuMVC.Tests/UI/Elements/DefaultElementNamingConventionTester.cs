using FubuCore.Reflection;
using FubuMVC.Core.UI.Elements;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Elements
{
    [TestFixture]
    public class DefaultElementNamingConventionTester
    {
        [Test]
        public void get_name_for_immediate_property_of_view_model()
        {
            new DefaultElementNamingConvention()
                .GetName(null, ReflectionHelper.GetAccessor<AddressViewModel>(x => x.ShouldShow))
                .ShouldEqual("ShouldShow");
        }

        [Test]
        public void get_name_for_nested_collection_property_of_the_view_model()
        {
            new DefaultElementNamingConvention()
                .GetName(null, ReflectionHelper.GetAccessor<AddressViewModel>(x => x.Localities[0].ZipCode))
                .ShouldEqual("Localities[0]ZipCode");
        }

        [Test]
        public void get_name_for_nested_property_of_the_view_model()
        {
            new DefaultElementNamingConvention()
                .GetName(null, ReflectionHelper.GetAccessor<AddressViewModel>(x => x.Address.City))
                .ShouldEqual("AddressCity");
        }
    }
}