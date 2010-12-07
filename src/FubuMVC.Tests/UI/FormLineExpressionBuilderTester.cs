using System;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.UI.Security;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.View;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class FormLineExpressionBuilderTester
    {
        [SetUp]
        public void SetUp()
        {
            tags = new StubTagGenerator<ViewModel>();
            _expression = null;

            page = MockRepository.GenerateMock<IFubuPage<ViewModel>>();
            page.Stub(x => x.Get<ITagGenerator<ViewModel>>()).Return(tags);
            page.Stub(x => x.Model).Return(new ViewModel());

            fieldAccess = MockRepository.GenerateMock<IFieldAccessService>();
            page.Stub(x => x.Get<IFieldAccessService>()).Return(fieldAccess);
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
        private StubTagGenerator<ViewModel> tags;
        private IFubuPage<ViewModel> page;
        private IFieldAccessService fieldAccess;

        private FormLineExpression<ViewModel> theResultingExpression
        {
            get
            {
                if (_expression == null)
                {
                    _expression = page.Show(x => x.Name);
                }

                return _expression;
            }
        }

        private AccessRight rightsAre
        {
            set
            {
                fieldAccess.Expect(x => x.RightsFor(null))
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