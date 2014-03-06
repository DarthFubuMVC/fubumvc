using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.SparkModel
{
    public class BindingsAttacherTester : InteractionContext<BindingsAttacher>
    {
        private ISparkTemplate _template;
        private IAttachRequest<ISparkTemplate> _request;
        private TemplateRegistry<ISparkTemplate> _templates;
        private SparkTemplate _viewViewToken;

        protected override void beforeEach()
        {
            _templates = new TemplateRegistry<ISparkTemplate>();

            Assert.Fail("Redo");

//            _viewViewToken = new SparkTemplate(_template, new SparkViewEngine());
//            _template = new SparkTemplate("/App/Views/Fubu.spark", "/App/Views", ContentFolder.Application)
//            {
//            };
//
//            _templates.Register(_template);
//            Enumerable.Range(1, 5).Select(x => new SparkTemplate("{0}.spark".ToFormat(x), "b", "c"))
//                .Each(_templates.Register);


            _request = new AttachRequest<ISparkTemplate>
            {
                Template = _template,
                Logger = MockFor<ITemplateLogger>(),
            };

            MockFor<ISharedTemplateLocator>()
                .Expect(x => x.LocateBindings(ClassUnderTest.BindingsName, _template))
                .Return(_templates);

            Container.Inject<ITemplateRegistry<ISparkTemplate>>(_templates);
        }

        [Test]
        public void binds_templates_with_viewdescriptor_and_view_model_type()
        {
            ClassUnderTest.CanAttach(_request).ShouldBeTrue();
        }

        [Test]
        public void add_each_binding_to_the_descriptor()
        {
            ClassUnderTest.Attach(_request);

            _viewViewToken.Bindings.ShouldEqual(_templates);
            MockFor<ISharedTemplateLocator>().VerifyAllExpectations();
        }


        [Test]
        public void does_not_attempt_add_bindings_if_prior_processed()
        {
            _viewViewToken.AddBinding(MockFor<ISparkTemplate>());
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void logger_is_used()
        {
            ClassUnderTest.Attach(_request); 
            MockFor<ITemplateLogger>().AssertWasCalled(x => 
                x.Log(Arg.Is(_template), Arg<string>.Is.Anything), 
                x => x.Repeat.Times(_templates.Count()));
        }
    }
}