using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using FubuLocalization;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Security;
using FubuMVC.Core.UI.Tags;
using HtmlTags;
using HtmlTags.Extended.Attributes;

namespace FubuMVC.Core.UI.Forms
{
    public class FormLineExpression<T> : ITagSource, IHtmlString where T : class
    {
        private readonly ITagGenerator<T> _tags;
        private readonly ILabelAndFieldLayout _layout;

        private readonly HashSet<string> _groupByCssClasses =
            new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public HashSet<string> GroupByCssClasses
        {
            get { return _groupByCssClasses; }
        }

        private bool _isVisible = true;
        private readonly ElementRequest _request;
        private readonly IList<Action<ILabelAndFieldLayout, ElementRequest>> _alterations 
            = new List<Action<ILabelAndFieldLayout, ElementRequest>>();

        private AccessRight _editable = AccessRight.ReadOnly;
        private AccessRight _rights = AccessRight.All;

        public FormLineExpression(ITagGenerator<T> tags, ILabelAndFieldLayout layout,
                                  Expression<Func<T, object>> expression)
            : this(tags, layout, tags.GetRequest(expression))
        {

        }

        public FormLineExpression(ITagGenerator<T> tags, ILabelAndFieldLayout layout,
                                  ElementRequest request)
        {
            _tags = tags;
            _layout = layout;
            _request = request;

            _layout.LabelTag = tags.LabelFor(request);

            AlterLayout(x => GroupByCssClasses.Each(c =>
            {
                x.LabelTag.AddClass(c);
                x.BodyTag.AddClass(c);
            }));
        }

        public FormLineExpression<T> AlterLabel(Action<HtmlTag> alter)
        {
            alter(_layout.LabelTag);
            return this;
        }

        public FormLineExpression<T> GroupByClass(string cssClass)
        {
            GroupByCssClasses.Add(cssClass);
            return this;
        }

        public FormLineExpression<T> AlterBody(Action<HtmlTag> alter)
        {
            return AlterLayout((l, r) => alter(l.BodyTag));
        }

        public FormLineExpression<T> AlterLayout(Action<ILabelAndFieldLayout> alter)
        {
            return AlterLayout((l, r) => alter(l));
        }

        public FormLineExpression<T> AlterLayout(Action<ILabelAndFieldLayout, ElementRequest> alter)
        {
            _alterations.Add(alter);

            return this;
        }

        public FormLineExpression<T> Label(HtmlTag tag)
        {
            _layout.LabelTag = tag;
            return this;
        }

        public FormLineExpression<T> Editable(bool condition)
        {
            _editable = condition ? AccessRight.All : AccessRight.ReadOnly;

            return this;
        }

        public AccessRight Access()
        {
            return _rights;            
        }

        public FormLineExpression<T> Access(AccessRight rights)
        {
            _rights = rights;
            return this;
        }

        public AccessRight Editable()
        {
            return _editable;
        }

        public FormLineExpression<T> EditableForRole(params string[] roles)
        {
            return Editable(PrincipalRoles.IsInRole(roles));
        }

        public FormLineExpression<T> Visible(bool condition)
        {
            _isVisible = condition;
            return this;
        }

        public FormLineExpression<T> LabelId(string id)
        {
            _layout.LabelTag.Id(id);
            return this;
        }

        public FormLineExpression<T> BodyId(string id)
        {
            return AlterBody(tag => tag.Id(id));
        }

        public void Compile()
        {
            createBodyTag();
        }

        private void createBodyTag()
        {
            _layout.BodyTag = isEditable() ? _tags.InputFor(_request) : _tags.DisplayFor(_request);
            _alterations.Each(a => a(_layout, _request));
        }

        private bool isEditable()
        {
            return _editable.Write && _rights.Write;
        }

        public override string ToString()
        {
            if (!isVisible()) return string.Empty;

            createBodyTag();

            return _layout.ToString();
        }

        public string ToHtmlString()
        {
            return ToString();
        }

        private bool isVisible()
        {
            return _isVisible && _rights.Read;
        }

        public FormLineExpression<T> AddClassToBody(string className)
        {
            AlterBody(t => t.AddClass(className));
            return this;
        }

        public FormLineExpression<T> DisplayIfEmpty(StringToken token)
        {
            return DisplayIfEmpty(token.ToString());
        }

        public FormLineExpression<T> DisplayIfEmpty(string text)
        {
            AlterLayout((layout, request) =>
            {
                if (request.ValueIsEmpty())
                {
                    layout.BodyTag.Text(text);
                }
            });

            return this;
        }

        public FormLineExpression<T> NoAutoComplete()
        {
            AlterBody(tag => tag.NoAutoComplete());
            return this;
        }

        IEnumerable<HtmlTag> ITagSource.AllTags()
        {
            if (!isVisible()) return new HtmlTag[0];

            createBodyTag();

            return _layout.AllTags();
        }



    }
}