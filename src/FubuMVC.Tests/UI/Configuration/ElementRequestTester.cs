using FubuCore;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Security;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Configuration
{
    [TestFixture]
    public class ElementRequestTester
    {
        [Test]
        public void Holder_type_with_no_model()
        {
            var request = ElementRequest.For<Model1>(null, x => x.Child.Name);
            request.HolderType().ShouldEqual(typeof (Model1));
        }

        [Test]
        public void Holder_type_with_a_model_but_no_services()
        {
            var request = ElementRequest.For<Model1>(new SubModel1(), x => x.Child.Name);
            request.HolderType().ShouldEqual(typeof(SubModel1));
        }

        [Test]
        public void Holder_type_with_a_model_and_service_locator()
        {
            var services = MockRepository.GenerateMock<IServiceLocator>();


            var theModel = new Model1();
            var request = ElementRequest.For<Model1>(theModel, x => x.Child.Name, services);

            var resolver = MockRepository.GenerateMock<ITypeResolver>();
            services.Stub(x => x.GetInstance<ITypeResolver>()).Return(resolver);
            resolver.Stub(x => x.ResolveType(theModel)).Return(GetType());

            request.HolderType().ShouldEqual(GetType());
        }

        [Test]
        public void get_access_rights()
        {
            var services = MockRepository.GenerateMock<IServiceLocator>();
            var rightsService = MockRepository.GenerateMock<IFieldAccessService>();

            services.Stub(x => x.GetInstance<IFieldAccessService>()).Return(rightsService);

            var theModel = new Model1();
            var request = ElementRequest.For<Model1>(theModel, x => x.Child.Name, services);

            rightsService.Stub(x => x.RightsFor(request)).Return(AccessRight.ReadOnly);

            request.AccessRights().ShouldEqual(AccessRight.ReadOnly);
        }

        public class Model1
        {
            public Model2 Child { get; set; }
        }

        public class SubModel1 : Model1{}

        public class Model2
        {
            public string Name { get; set; }
        }
    }
}