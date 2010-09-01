using System;
using FubuMVC.UI;
using FubuMVC.UI.Configuration;
using FubuMVC.UI.Forms;
using FubuMVC.UI.Security;
using FubuMVC.UI.Tags;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class FormLineExpressionBuilderTester : InteractionContext<FormLineExpressionBuilder<ViewModel>>
    {
        protected override void beforeEach()
        {
            Services.Inject<ITagGenerator<ViewModel>>(new StubTagGenerator<ViewModel>());
            _expression = null;
        }

        private void theHtmlOutputShouldBeReadOnly()
        {
            theResultingExpression.ToString().ShouldContain("display");
        }

        private void theHtmlOutputShouldBeEditable()
        {
            theResultingExpression.ToString().ShouldContain("input");
        }

        private void noHtmlShouldBeRendered()
        {
            theResultingExpression.ToString().ShouldBeEmpty();
        }

        private FormLineExpression<ViewModel> _expression;
        private FormLineExpression<ViewModel> theResultingExpression
        {
            get
            {
                if (_expression == null)
                {
                    _expression = ClassUnderTest.Build(x => x.Name);
                }

                return _expression;
            }
        }

        private AccessRight rightsAre
        {
            set
            {
                MockFor<IFieldAccessService>().Expect(x => x.RightsFor(null))
                    .Constraints(Is.Matching<ElementRequest>(r => r.Accessor.Name == "Name"))
                    .Return(value);
            }
        }

        [Test]
        public void show_with_all_access_rights_should_be_visible_and_readonly()
        {
            rightsAre = AccessRight.All;

            theHtmlOutputShouldBeReadOnly();
        }

        [Test]
        public void show_with_read_only_access_rights_should_be_visible_and_readonly()
        {
            rightsAre = AccessRight.ReadOnly;

            theHtmlOutputShouldBeReadOnly();
        }

        [Test]
        public void show_with_no_access_should_not_be_visible()
        {
            rightsAre = AccessRight.None;
            noHtmlShouldBeRendered();
        }

        [Test]
        public void edit_with_all_access_rights_should_be_visible_and_editable()
        {
            rightsAre = AccessRight.All;
            theResultingExpression.Editable(true);
            
            
            theHtmlOutputShouldBeEditable();
        }

        [Test]
        public void edit_with_readonly_access_rights_should_be_visible_and_readonly()
        {
            rightsAre = AccessRight.ReadOnly;
            theResultingExpression.Editable(true);

            

            theHtmlOutputShouldBeReadOnly();
        }

        [Test]
        public void edit_with_no_access_rights_should_be_hidden()
        {
            rightsAre = AccessRight.None;
            theResultingExpression.Editable(true);

            
            
            noHtmlShouldBeRendered();
        }

        [Test]
        public void should_be_a_label()
        {
            rightsAre = AccessRight.All;

            theResultingExpression.ToString().ShouldContain("<span class=\"label\">Name</span>");
        }
    }

    public class ViewModel
    {
        public string Name { get; set; }
    }
}