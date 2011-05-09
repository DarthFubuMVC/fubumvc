using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FubuCore.Binding;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BasicTypeConverterTester
    {
        private ValueConverterRegistry _registry;
        private TypeDescriptorConverterFamily _typeDescriptorConverterFamily;
        private PropertyInfo _property;
        private IPropertyContext _context;
        private string _propertyValue;

        private class PropertyHolder{public string Property { get; set; }}

        [SetUp]
        public void SetUp()
        {
            _registry = new ValueConverterRegistry(new IConverterFamily[0]);
            _property = typeof(PropertyHolder).GetProperty("Property");
            _typeDescriptorConverterFamily = _registry.Families.SingleOrDefault(cf =>
                cf.Matches(_property)) as TypeDescriptorConverterFamily;
            _typeDescriptorConverterFamily.ShouldNotBeNull();

            _context = MockRepository.GenerateMock<IPropertyContext>();
            _context.Stub(x => x.Property).Return(_property);
            _propertyValue = "some value";
            _context.Expect(c => c.PropertyValue).Return(_propertyValue).Repeat.Times(3);
        }

        [Test]
        public void should_match_property()
        {
            _typeDescriptorConverterFamily.Matches(_property).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_on_exception()
        {
            _typeDescriptorConverterFamily.Matches(null).ShouldBeFalse();
        }

        [Test]
        public void should_build()
        {
            ValueConverter converter = _typeDescriptorConverterFamily.Build(_registry, _property);
            converter.Convert(_context).ShouldEqual(_propertyValue);
            _context.VerifyAllExpectations();
        }
    }

    [TestFixture]
    public class USCultureNumericFamilyTester
    {
        private ValueConverterRegistry _registry;
        private NumericTypeFamily _numericTypeFamily;
        private PropertyInfo _property;
        private IPropertyContext _context;
        private string _propertyValue;

        private class PropertyHolder { public decimal Property { get; set; } }

        [SetUp]
        public void SetUp()
        {
            _registry = new ValueConverterRegistry(new IConverterFamily[0]);
            _property = typeof(PropertyHolder).GetProperty("Property");
            _numericTypeFamily = _registry.Families.FirstOrDefault(cf =>
                cf.Matches(_property)) as NumericTypeFamily;
            _numericTypeFamily.ShouldNotBeNull();

            _context = MockRepository.GenerateMock<IPropertyContext>();
            _context.Stub(x => x.Property).Return(_property);
            _propertyValue = "1,000.001";
            _context.Expect(c => c.PropertyValue).Return(_propertyValue).Repeat.Times(4);
        }

        [Test]
        public void should_match_property()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("en-us")))
                _numericTypeFamily.Matches(_property).ShouldBeTrue();
        }

        [Test]
        public void should_build()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("en-us")))
            {
                ValueConverter converter = _numericTypeFamily.Build(_registry, _property);
                converter.Convert(_context).ShouldEqual(1000.001m);
                _context.VerifyAllExpectations();
            }
        }
    }

    [TestFixture]
    public class GermanCultureNumericFamilyTester
    {
        private ValueConverterRegistry _registry;
        private NumericTypeFamily _numericTypeFamily;
        private PropertyInfo _property;
        private IPropertyContext _context;
        private string _propertyValue;

        private class PropertyHolder { public Decimal Property { get; set; } }

        [SetUp]
        public void SetUp()
        {
            _registry = new ValueConverterRegistry(new IConverterFamily[0]);
            _property = typeof(PropertyHolder).GetProperty("Property");
            _numericTypeFamily = _registry.Families.FirstOrDefault(cf =>
                cf.Matches(_property)) as NumericTypeFamily;
            _numericTypeFamily.ShouldNotBeNull();

            _context = MockRepository.GenerateMock<IPropertyContext>();
            _context.Stub(x => x.Property).Return(_property);
            _propertyValue = "1.000,001";
            _context.Expect(c => c.PropertyValue).Return(_propertyValue).Repeat.Times(4);
        }

        [Test]
        public void should_match_property()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("de-DE")))
                _numericTypeFamily.Matches(_property).ShouldBeTrue();
        }

        [Test]
        public void should_build()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("de-DE")))
            {
                ValueConverter converter = _numericTypeFamily.Build(_registry, _property);
                converter.Convert(_context).ShouldEqual(1000.001m);
                _context.VerifyAllExpectations();
            }
        }
    }
}