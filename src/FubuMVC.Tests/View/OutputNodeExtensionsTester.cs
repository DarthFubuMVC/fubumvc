using System;
using FubuMVC.Core;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class OutputNodeExtensionsTester
    {


        [Test]
        public void has_view_is_false_when_it_is_empty()
        {
            var node = new OutputNode(typeof(Address));
            node.HasView(Always.Flyweight).ShouldBeFalse();
        }

        [Test]
        public void has_view_is_positive_with_always()
        {
            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<ITemplateFile>();
            node.AddView(viewToken, null);

            node.HasView(Always.Flyweight).ShouldBeTrue();
        }

        [Test]
        public void has_view_negative_when_there_is_a_view_but_it_has_different_conditions()
        {
            var condition = new FakeConditional();

            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<ITemplateFile>();
            node.AddView(viewToken, condition);

            node.HasView(Always.Flyweight).ShouldBeFalse();
        }

        [Test]
        public void has_view_positive_with_different_conditional()
        {
            var condition = new FakeConditional();

            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<ITemplateFile>();
            node.AddView(viewToken, condition);

            node.HasView(condition).ShouldBeTrue();
        }
       

        public class FakeConditional : IConditional
        {
            public bool ShouldExecute(IFubuRequestContext context)
            {
                throw new NotImplementedException();
            }
        }

        public class Address
        {
            public Address()
            {
                StateOrProvince = string.Empty;
                Country = string.Empty;
                AddressType = string.Empty;
            }

            public int Order { get; set; }

            public bool IsActive { get; set; }

            public string AddressType { get; set; }

            public string Address1 { get; set; }

            public string Address2 { get; set; }

            public string City { get; set; }

            public string StateOrProvince { get; set; }

            public string Country { get; set; }

            public string PostalCode { get; set; }

            //[Required]
            //public TimeZoneInfo TimeZone { get; set; }


            public DateTime DateEntered { get; set; }

            public ColorEnum Color { get; set; }
            public Guid Guid { get; set; }
        }

        public enum ColorEnum
        {
            red,
            blue,
            green
        }
    }
}