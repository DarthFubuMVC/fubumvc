using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using FubuCore.Binding;
using FubuCore.Reflection;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class PassthroughBinderTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void matches_by_property_type_positive()
        {
            var binder = new PassthroughConverter<HttpPostedFileBase>();
            binder.Matches(property(x => x.File)).ShouldBeTrue();
        }

        [Test]
        public void matches_by_property_type_negative()
        {
            var binder = new PassthroughConverter<HttpPostedFileBase>();
            binder.Matches(property(x => x.File2)).ShouldBeFalse();
        }

        [Test]
        public void build_passes_through()
        {
            var binder = new PassthroughConverter<HttpPostedFileBase>();
            IBindingContext context = MockRepository.GenerateMock<IBindingContext>();
            context.Expect(c => c.PropertyValue).Return(new object());
            ValueConverter converter = binder.Build(MockRepository.GenerateStub<IValueConverterRegistry>(), property(x => x.File));
            converter(context);
            context.VerifyAllExpectations();
        }

        private PropertyInfo property(Expression<Func<ModelWithHttpPostedFileBase, object>> expression)
        {
            return ReflectionHelper.GetProperty(expression);
        }
    }

    public class ModelWithHttpPostedFileBase
    {
        public HttpPostedFileBase File { get; set; }
        public string File2 { get; set; }
    }
}