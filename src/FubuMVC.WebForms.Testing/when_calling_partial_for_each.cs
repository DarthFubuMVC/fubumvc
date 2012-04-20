using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.View;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.WebForms.Testing
{
    [TestFixture]
    public class when_calling_partial_for_each
    {
        private IFubuPage<InputModel> _page;
        private InputModel _model;
        private IPartialRenderer _renderer;
        private IPartialViewTypeRegistry _viewTypeRegistry;
        private TagGenerator<InputModel> _tags;

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateMock<IFubuPage<InputModel>>();
            _renderer = MockRepository.GenerateStub<IPartialRenderer>();
            var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            var namingConvention = MockRepository.GenerateStub<IElementNamingConvention>();
            _tags = new TagGenerator<InputModel>(new TagProfileLibrary(), namingConvention,
                                                 serviceLocator);
            
            _viewTypeRegistry = MockRepository.GenerateStub<IPartialViewTypeRegistry>();
            serviceLocator.Stub(s => s.GetInstance<IPartialViewTypeRegistry>()).Return(_viewTypeRegistry);

            var inMemoryFubuRequest = new InMemoryFubuRequest();
            inMemoryFubuRequest.Set(new InputModel());

            _page.Stub(s => s.Get<IFubuRequest>()).Return(inMemoryFubuRequest);
            
            _model = new InputModel{Partials=new List<PartialModel>{new PartialModel()}};
            _page.Expect(p => p.Get<ITagGenerator<InputModel>>()).Return(_tags);
            _page.Expect(p => p.Model).Return(_model);
            _page.Expect(p => p.Get<IPartialRenderer>()).Return(_renderer);
            _page.Expect(p => p.ServiceLocator).Return(serviceLocator);
        }

        [Test]
        public void should_return_expression_without_calling_using()
        {
            _viewTypeRegistry.Stub(r => r.HasPartialViewTypeFor<PartialModel>()).Return(false);

            _page.PartialForEach(x=>x.Partials);
            
            _page.VerifyAllExpectations();
            _viewTypeRegistry.AssertWasCalled(r => r.HasPartialViewTypeFor<PartialModel>());
            _viewTypeRegistry.AssertWasNotCalled(r => r.GetPartialViewTypeFor<PartialModel>());
            _renderer.AssertWasNotCalled(r => r.CreateControl(typeof(when_registering_partial_view_types.PartialView)));
        }

        [Test]
        public void should_return_expression_after_calling_using()
        {
            _viewTypeRegistry.Stub(r => r.HasPartialViewTypeFor<PartialModel>()).Return(true);
            _viewTypeRegistry.Stub(r => r.GetPartialViewTypeFor<PartialModel>()).Return(typeof (when_registering_partial_view_types.PartialView));

            _page.PartialForEach(x => x.Partials);

            _page.VerifyAllExpectations();
            _viewTypeRegistry.AssertWasCalled(r => r.HasPartialViewTypeFor<PartialModel>());
            _viewTypeRegistry.AssertWasCalled(r => r.GetPartialViewTypeFor<PartialModel>());
            _renderer.AssertWasCalled(r => r.CreateControl(typeof(when_registering_partial_view_types.PartialView)));
        }

        public class PartialModel { }
        public class InputModel { public IList<PartialModel> Partials { get; set; } }
    }
}