using System.Collections.Generic;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Configuration;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
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
        public void get_name_for_nested_property_of_the_view_model()
        {
            new DefaultElementNamingConvention()
                .GetName(null, ReflectionHelper.GetAccessor<AddressViewModel>(x => x.Address.City))
                .ShouldEqual("AddressCity");
        }

        [Test]
        public void get_name_for_nested_collection_property_of_the_view_model()
        {
            new DefaultElementNamingConvention()
                .GetName(null, ReflectionHelper.GetAccessor<AddressViewModel>(x => x.Localities[0].ZipCode))
                .ShouldEqual("Localities[0]ZipCode");
        }
    }

    public class AddressViewModel
    {
        public Address Address { get; set; }
        public bool ShouldShow { get; set; }
        public IList<LocalityViewModel> Localities { get; set; }
    }

    public class LocalityViewModel
    {
        public string ZipCode { get; set; }
        public string CountyName { get; set; }
    }
}