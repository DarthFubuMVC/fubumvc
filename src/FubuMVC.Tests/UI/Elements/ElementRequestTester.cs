using FubuCore;
using FubuCore.Formatting;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Elements
{
    [TestFixture]
    public class ElementRequestTester
    {
        [Test]
        public void Holder_type_with_no_model()
        {
            var request = ElementRequest.For<Model1>(null, x => x.Child.Name);
            request.HolderType().ShouldEqual(typeof(Model1));
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
            var request = ElementRequest.For<Model1>(theModel, x => x.Child.Name);
            request.Attach(services);

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
            var request = ElementRequest.For<Model1>(theModel, x => x.Child.Name);
            request.Attach(services);

            rightsService.Stub(x => x.RightsFor(request)).Return(AccessRight.ReadOnly);

            request.AccessRights().ShouldEqual(AccessRight.ReadOnly);
        }

        [Test]
        public void StringValue_delegates_to_Stringifier()
        {
            var stringifier = new Stringifier();

            stringifier.AddStrategy(new StringifierStrategy{
                Matches = r => true,
                StringFunction = r => "*" + r.RawValue + "*"
            });

            var services = new InMemoryServiceLocator();
            services.Add(stringifier);
            services.Add<IDisplayFormatter>(new DisplayFormatter(services, stringifier));

            var request = ElementRequest.For<Model1>(new Model1{Child = new Model2{Name = "Little Lindsey"}}, x => x.Child.Name);
            request.Attach(services);

            request.StringValue().ShouldEqual("*Little Lindsey*");
        
        }



        public class Model1
        {
            public Model2 Child { get; set; }
        }

        public class SubModel1 : Model1 { }

        public class Model2
        {
            public string Name { get; set; }
        }
    }
}