using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.View.Rendering;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Core.View.Testing
{
    [TestFixture]
    public class OutputNodeExtensionsTester
    {

        [Test]
        public void add_view()
        {
            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<IViewToken>();

            var viewNode = node.AddView(viewToken);
            node.Writers.ShouldContain(viewNode);

            viewNode.View.ShouldBeTheSameAs(viewToken);
        }

        // Sometimes, ugly code just isn't worth the effort to fix it.
        [Test]
        public void add_view_with_condition()
        {
            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<IViewToken>();
            viewToken.Stub(x => x.ViewModel).Return(typeof(Address));
            viewToken.Stub(x => x.ToViewFactoryObjectDef()).Return(
                ObjectDef.ForValue(MockRepository.GenerateMock<IViewFactory>()));

            var viewNode = node.AddView(viewToken, typeof(FakeConditional));

            viewNode.As<IContainerModel>().ToObjectDef()
                .FindDependencyDefinitionFor<IConditional>()
                .Type
                .ShouldEqual(typeof(FakeConditional));
        }


        [Test]
        public void has_view_is_false_when_it_is_empty()
        {
            var node = new OutputNode(typeof(Address));
            node.HasView(typeof(Always)).ShouldBeFalse();
        }

        [Test]
        public void has_view_is_positive_with_always()
        {
            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<IViewToken>();
            node.AddView(viewToken);

            node.HasView(typeof(Always)).ShouldBeTrue();
        }

        [Test]
        public void has_view_negative_when_there_is_a_view_but_it_has_different_conditions()
        {
            Type conditionType = typeof(FakeConditional);

            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<IViewToken>();
            node.AddView(viewToken, conditionType);

            node.HasView(typeof(Always)).ShouldBeFalse();
        }

        [Test]
        public void has_view_positive_with_different_conditional()
        {
            Type conditionType = typeof(FakeConditional);

            var node = new OutputNode(typeof(Address));
            var viewToken = MockRepository.GenerateMock<IViewToken>();
            node.AddView(viewToken, conditionType);

            node.HasView(conditionType).ShouldBeTrue();
        }

        public class FakeConditional : IConditional
        {
            public bool ShouldExecute()
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
            red, blue, green
        }
    }
}