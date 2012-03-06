using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class ViewContentDisposerTester : InteractionContext<ViewContentDisposer>
    {
        private NestedOutput _nestedOutput;
        private IFubuSparkView _view;
        protected override void beforeEach()
        {
            MockFor<ViewOutput>();
            _view = MockFor<IFubuSparkView>();
            _view.Stub(x => x.Content).PropertyBehavior();
            _view.Content = new Dictionary<string, TextWriter>();
            _nestedOutput = new NestedOutput();
            Services.Inject(_nestedOutput);
        }

        [Test]
        public void applies_if_nestedoutput_is_not_active()
        {
            ClassUnderTest.Applies(_view).ShouldBeTrue();
            _nestedOutput.SetWriter(() => new StringWriter());
            ClassUnderTest.Applies(_view).ShouldBeFalse();
        }

        [Test]
        public void after_render_the_modified_view_clears_its_content_dictionary()
        {
            var view = ClassUnderTest.Modify(_view).As<IFubuSparkView>();
            var repository = new MockRepository();
            var writer1 = repository.DynamicMock<TextWriter>();
            var writer2 = repository.DynamicMock<TextWriter>();
            var writer3 = repository.DynamicMock<TextWriter>();
            using (repository.Record())
            {
                writer1.Expect(x => x.Close());
                writer2.Expect(x => x.Close());
                writer3.Expect(x => x.Close());
            }
            repository.ReplayAll();
            view.Content.Add("view", writer1);
            view.Content.Add("top", writer2);
            view.Content.Add("bottom", writer3);

            view.Content.ShouldHaveCount(3);
            view.Render();
            view.Content.ShouldHaveCount(0);

            writer1.VerifyAllExpectations();
            writer2.VerifyAllExpectations();
            writer3.VerifyAllExpectations();

        }
    }
}