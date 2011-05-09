using System.Reflection;
using System.Web;
using FubuCore.Binding;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ASPNetObjectConversionFamilyTester
    {
        private ValueConverterRegistry _registry;
        private ASPNetObjectConversionFamily _aspNetObjectConversionFamily;
        private PropertyInfo _property;
        private IPropertyContext _context;
        private object _propertyValue;

        [SetUp]
        public void SetUp()
        {
            _registry = new ValueConverterRegistry(new IConverterFamily[0]);
            _property = typeof(HttpRequestBase).GetProperty("Browser");
            _aspNetObjectConversionFamily = _registry.Families.SingleOrDefault(cf => 
                cf.Matches(_property)) as ASPNetObjectConversionFamily;
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