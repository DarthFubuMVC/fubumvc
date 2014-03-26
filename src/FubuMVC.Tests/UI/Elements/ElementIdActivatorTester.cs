using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Elements;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Elements
{
    [TestFixture]
    public class ElementIdActivatorTester : InteractionContext<ElementIdActivator>
    {
        private Accessor _accessor;
        private class Person
        {
            public string FirstName { get; set; }
        }

        protected override void beforeEach()
        {
            Expression<Func<Person, object>> expression = p => p.FirstName;
            _accessor = expression.ToAccessor();

            MockFor<IElementNamingConvention>()
                .Stub(x => x.GetName(typeof (Person), _accessor))
                .Return("FirstName");
        }

        [Test]
        public void Returns_existing_element_id()
        {
            var elementRequest = new ElementRequest(_accessor)
            {
                ElementId = "Person[0]FirstName"
            };

            ClassUnderTest.Activate(elementRequest);

            elementRequest.ElementId.ShouldEqual("Person[0]FirstName");
        }


        [Test]
        public void Returns_element_id_from_naming_convention()
        {
            var elementRequest = new ElementRequest(_accessor);

            ClassUnderTest.Activate(elementRequest);

            elementRequest.ElementId.ShouldEqual("FirstName");
        }
    }
}