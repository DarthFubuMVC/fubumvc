using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using FubuCore;

namespace FubuMVC.Tests.View.New
{
    [TestFixture]
    public class ViewNodeTester : InteractionContext<ViewNode>
    {
        [Test]
        public void resource_type_is_the_view_model_from_the_view()
        {
            MockFor<IViewToken>().Stub(x => x.ViewModel).Return(GetType());

            ClassUnderTest.ResourceType.ShouldEqual(GetType());
        }

        [Test]
        public void mime_type_is_only_html_FOR_NOW()
        {
            ClassUnderTest.Mimetypes.Single().ShouldEqual(MimeType.Html.Value);
        }

        [Test]
        public void build_object_def()
        {
            MockFor<IViewToken>().Stub(x => x.ViewModel).Return(typeof(SomeResource));

            var viewFactoryDef = ObjectDef.ForValue(MockRepository.GenerateMock<IViewFactory>());
            MockFor<IViewToken>().Stub(x => x.ToViewFactoryObjectDef())
                .Return(viewFactoryDef);

             ClassUnderTest.As<IContainerModel>()
                .ToObjectDef(DiagnosticLevel.None)
                .FindDependencyDefinitionFor<IMediaWriter<SomeResource>>()
                .FindDependencyDefinitionFor<IViewFactory>()
                .ShouldBeTheSameAs(viewFactoryDef);
        }
    }

    public class SomeResource{}

    
}