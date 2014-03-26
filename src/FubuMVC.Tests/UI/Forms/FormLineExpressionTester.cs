using System;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;
using FubuCore;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.UI.Security;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using System.Linq;
using FubuCore.Reflection;

namespace FubuMVC.Tests.UI.Forms
{
    [TestFixture]
    public class FormLineExpressionTester
    {
        private DefinitionListFieldChrome layout;
        private FormLineExpression<ViewModel> expression;

        [SetUp]
        public void SetUp()
        {
            layout = new DefinitionListFieldChrome();
            expression = new FormLineExpression<ViewModel>(layout, new StubElementGenerator<ViewModel>(layout), x => x.Name);
        }

        [Test]
        public void alter_layout_with_an_element_request()
        {
            expression.AlterLayout((l, r) =>
            {
                l.LabelTag.Text("Prop:  " + r.Accessor.Name);
            });

            expression.Compile();

            layout.LabelTag.Text().ShouldEqual("Prop:  Name");
        }

        [Test]
        public void add_class_to_body()
        {
            expression.AddClassToBody("class1");
            expression.Compile();
            layout.BodyTag.HasClass("class1").ShouldBeTrue();
        }

        [Test]
        public void display_if_empty_by_text()
        {
            expression.DisplayIfEmpty("default text");
            expression.Compile();
            layout.BodyTag.Text().ShouldEqual("default text");
        }

        [Test]
        public void no_autocomplete()
        {
            expression.NoAutoComplete();
            expression.Compile();
            layout.BodyTag.ToString().ShouldEqual("<span autocomplete=\"off\" class=\"display\">Name</span>");
        }

        public class MyTokenClass
        {

        }

        [Test]
        public void display_if_empty_by_localized_string_token()
        {
            var token = new MyTokenClass();
            expression.DisplayIfEmpty(token);
            expression.Compile();


            layout.BodyTag.Text().ShouldEqual(token.ToString());
        }

        [Test]
        public void alter_the_body()
        {
            expression.AlterBody(b => b.Text("the text"));
            expression.Compile();
            layout.BodyTag.Text().ShouldEqual("the text");
        }

        [Test]
        public void group_by_css_classes()
        {
            expression.GroupByClass("class1");
            expression.Compile();

            layout.BodyTag.HasClass("class1").ShouldBeTrue();
            layout.LabelTag.HasClass("class1").ShouldBeTrue();
        }


        [Test]
        public void can_change_label_attributes_after_setting_label_text()
        {
            var tag = expression
                .AlterLayout(x => x.LabelTag.Text("bar"))
                .AlterLabel(label => label.AddClass("foo"));

            tag.AlterLayout(x => x.LabelTag.HasClass("foo").ShouldBeTrue());
        }

        [Test]
        public void puts_css_class_on_the_label_and_body()
        {
            expression.GroupByClass("groupByClass");
            expression.ToString();
            layout.BodyTag.GetClasses().ShouldContain("groupByClass");
            layout.LabelTag.GetClasses().ShouldContain("groupByClass");
        }



        [Test]
        public void has_label_by_default()
        {
            expression.AlterLabel(l =>
            {
                l.ShouldNotBeNull();
                l.HasClass("label").ShouldBeTrue();
            });
        }

        [Test]
        public void label_id()
        {
            expression.LabelId("id1");
            layout.LabelTag.Id().ShouldEqual("id1");
        }

        [Test]
        public void body_id()
        {
            expression.BodyId("id2");
            expression.Compile();
            layout.BodyTag.Id().ShouldEqual("id2");
        }

        [Test]
        public void replace_label_with_text()
        {
            HtmlTag newLabel = new HtmlTag("span").Text("123");
            expression.Label(newLabel);

            expression.AlterLabel(l =>
            {
                l.ShouldBeTheSameAs(newLabel);
            });
        }

        [Test]
        public void editable_is_read_only_by_default()
        {
            expression.Editable().ShouldEqual(AccessRight.ReadOnly);
        }

        [Test]
        public void access_is_all_by_default()
        {
            expression.Access().ShouldEqual(AccessRight.All);
        }

        [Test]
        public void display_by_default_in_the_to_string()
        {
            string html = expression.ToString();
            html.ShouldContain("display");
            html.ShouldNotContain("input");
        }

        [Test]
        public void edit_if_the_condition_is_true()
        {
            string html = expression.Editable(true).ToString();
            html.ShouldContain("input");
            html.ShouldNotContain("display");
        }

        [Test]
        public void do_not_edit_if_the_condition_is_true_but_the_access_rights_are_readonly()
        {
            var html = expression.Editable(true).Access(AccessRight.ReadOnly).ToString();
            html.ShouldContain("display");
            html.ShouldNotContain("input");
        }

        [Test]
        public void do_not_display_if_the_editable_condition_is_true_but_the_access_rights_are_none()
        {
            expression.Visible(true);
            expression.Editable(true);
            expression.Access(AccessRight.None);

            expression.ToString().ShouldBeEmpty();
        }


        [Test]
        public void do_not_return_any_tags_if_the_editable_condition_is_true_but_the_access_rights_are_none()
        {
            expression.Visible(true);
            expression.Editable(true);
            expression.Access(AccessRight.None);

            expression.As<ITagSource>().AllTags().Any().ShouldBeFalse();
        }



        [Test]
        public void return_tags_if_the_editable_condition_is_true_and_the_access_rights_is_some()
        {
            expression.Visible(true);
            expression.Editable(true);
            expression.Access(AccessRight.All);

            expression.As<ITagSource>().AllTags().Any().ShouldBeTrue();
        }

        [Test]
        public void edit_if_the_user_has_roles_positive()
        {
            Thread.CurrentPrincipal = new System.Security.Principal.GenericPrincipal(new GenericIdentity("somebody"),
                                                                           new string[] { "admin" });

            expression.EditableForRole("admin").ToString().ShouldContain("input");
        }

        [Test]
        public void edit_if_the_user_does_not_have_a_role()
        {
            PrincipalRoles.SetCurrentRolesForTesting();

            expression.EditableForRole("admin").ToString().ShouldNotContain("input");
        }

        [Test]
        public void do_not_display_if_the_access_rights_are_none()
        {
            expression.Access(AccessRight.None);
            expression.ToString().ShouldBeEmpty();
        }


        [Test]
        public void display_if_the_condition_is_false()
        {
            string html = expression.Editable(false).ToString();
            html.ShouldNotContain("input");
            html.ShouldContain("display");
        }

        [Test]
        public void to_string_is_non_blank_by_default()
        {
            expression.ToString().Length.ShouldBeGreaterThan(0);
        }

        [Test]
        public void to_string_is_non_blank_when_the_visible_condition_is_true()
        {
            expression.Visible(true).ToString().Length.ShouldBeGreaterThan(0);
        }

        [Test]
        public void to_string_is_blank_when_the_visible_condition_is_false()
        {
            expression.Visible(false).ToString().ShouldEqual(string.Empty);
        }

        public class ViewModel
        {
            public string Name { get; set; }
        }
    }

    public class StubElementGenerator<T> : IElementGenerator<T> where T : class
    {
        private readonly IFieldChrome _layout;

        public StubElementGenerator(IFieldChrome layout)
        {
            _layout = layout;
        }

        public HtmlTag LabelFor(Expression<Func<T, object>> expression, string profile = null, T model = default(T))
        {
            return new HtmlTag("span").AddClass("label").Text(expression.ToAccessor().Name);
        }

        public HtmlTag InputFor(Expression<Func<T, object>> expression, string profile = null, T model = default(T))
        {
            return new HtmlTag("span").AddClass("input").Text(expression.ToAccessor().Name);
        }

        public HtmlTag DisplayFor(Expression<Func<T, object>> expression, string profile = null, T model = default(T))
        {
            return new HtmlTag("span").AddClass("display").Text(expression.ToAccessor().Name);
        }

        public HtmlTag InputFor(ElementRequest request, string profile = null, T model = default(T))
        {
            return new HtmlTag("span").AddClass("input").Text(request.Accessor.Name);
        }

        public HtmlTag LabelFor(ElementRequest request, string profile = null, T model = default(T))
        {
            return new HtmlTag("span").AddClass("label").Text(request.Accessor.Name);
        }

        public HtmlTag DisplayFor(ElementRequest request, string profile = null, T model = default(T))
        {
            return new HtmlTag("span").AddClass("display").Text(request.Accessor.Name);
        }

        public T Model { get; set; }
        public IFieldChrome Chrome()
        {
            return _layout;
        }
    }
}