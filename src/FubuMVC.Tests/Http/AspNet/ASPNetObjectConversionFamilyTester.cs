using System.Reflection;
using System.Web;
using FubuCore.Binding;
using FubuMVC.Core.Http.AspNet;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;
using FubuCore;

namespace FubuMVC.Tests.Http.AspNet
{
    [TestFixture]
    public class ASPNetObjectConversionFamilyTester
    {
        private ValueConverterRegistry _registry;
        private AspNetObjectConversionFamily _aspNetObjectConversionFamily;
        private PropertyInfo _property;
        private IPropertyContext _context;
        private object _propertyValue;

        [SetUp]
        public void SetUp()
        {
            _registry =
                StructureMapContainerFacility.GetBasicFubuContainer().GetInstance<IValueConverterRegistry>().As
                    <ValueConverterRegistry>();
            _property = typeof(HttpRequestBase).GetProperty("Browser");
            _aspNetObjectConversionFamily = _registry.Families.SingleOrDefault(cf => 
                cf.Matches(_property)) as AspNetObjectConversionFamily;
            _aspNetObjectConversionFamily.ShouldNotBeNull();

            _context = MockRepository.GenerateMock<IPropertyContext>();
            _propertyValue = new object();
            _context.Expect(c => c.PropertyValue).Return(_propertyValue);
        }

        [Test]
        public void should_build()
        {
            ValueConverter converter = _aspNetObjectConversionFamily.Build(_registry, _property);
            converter.Convert(_context).ShouldEqual(_propertyValue);
            _context.VerifyAllExpectations();
        }
    }
}