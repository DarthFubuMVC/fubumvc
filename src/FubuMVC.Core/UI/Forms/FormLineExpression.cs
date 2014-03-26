
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Security;
using HtmlTags;
using FubuCore.Reflection;

namespace FubuMVC.Core.UI.Forms
{
    public class FormLineExpression<T> : ITagSource, IHtmlString where T : class
    {
        private readonly IElementGenerator<T> _tags;
        private readonly IFieldChrome _chrome;
        private bool _isVisible = true;
        private readonly ElementRequest _request;
        private AccessRight _editable = AccessRight.ReadOnly;
        private AccessRight _rights = AccessRight.All;

        private readonly HashSet<string> _groupByCssClasses =
            new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IList<Action<IFieldChrome, ElementRequest>> _alterations 
            = new List<Action<IFieldChrome, ElementRequest>>();




        public FormLineExpression(IFieldChrome chrome, IElementGenerator<T> tags, Expression<Func<T, object>> expression)
            : this(chrome, tags, new ElementRequest(expression.ToAccessor()))
        {

        }

        public FormLineExpression(IFieldChrome chrome, IElementGenerator<T> tags, ElementRequest request)
        {
            _tags = tags;
            _chrome = chrome;
            _request = request;

            _chrome.LabelTag = tags.LabelFor(request);

            AlterLayout(x => GroupByCssClasses.Each(c =>
            {
                x.LabelTag.AddClass(c);
                x.BodyTag.AddClass(c);
            }));
        }



        public HashSet<string> GroupByCssClasses
        {
            get { return _groupByCssClasses; }
        }

        public FormLineExpression<T> AlterLabel(Action<HtmlTag> alter)
        {
            alter(_chrome.LabelTag);
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

        public FormLineExpression<T> AlterLayout(Action<IFieldChrome> alter)
        {
            return AlterLayout((l, r) => alter(l));
        }

        public FormLineExpression<T> AlterLayout(Action<IFieldChrome, ElementRequest> alter)
        {
            _alterations.Add(alter);

            return this;
        }

        public FormLineExpression<T> Label(HtmlTag tag)
        {
            _chrome.LabelTag = tag;
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
            _chrome.LabelTag.Id(id);
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
            if(isEditable())
            {
                _chrome.BodyTag = _tags.InputFor(_request);
            }
            else
            {
                _chrome.BodyTag = _tags.DisplayFor(_request);
            }
            _alterations.Each(a => a(_chrome, _request));
        }

        private bool isEditable()
        {
            return _editable.Write && _rights.Write;
        }

        public override string ToString()
        {
            if (!isVisible()) return string.Empty;

            createBodyTag();

            return _chrome.Render();
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

        public FormLineExpression<T> DisplayIfEmpty(object token)
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

        IEnumerable<HtmlTag> ITagSource.AllTags()
        {
            if (!isVisible()) return new HtmlTag[0];

            createBodyTag();

            return _chrome.AllTags();
        }

        public FormLineExpression<T> NoAutoComplete()
        {
            AlterBody(x => x.Attr("autocomplete", "off"));
            return this;
        }
    }
}
