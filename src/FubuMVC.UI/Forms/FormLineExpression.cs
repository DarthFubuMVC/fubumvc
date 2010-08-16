using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuMVC.UI.Configuration;
using FubuMVC.UI.Tags;
using HtmlTags;

namespace FubuMVC.UI.Forms
{
    // TODO:  Revisit this, and wrap some serious tests around it
    public class FormLineExpression<T> : ITagSource where T : class
    {
        private readonly ITagGenerator<T> _tags;
        private readonly ILabelAndFieldLayout _layout;
        private readonly Expression<Func<T, object>> _expression;
        private readonly HashSet<string> _groupByCssClasses = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        public HashSet<string> GroupByCssClasses { get { return _groupByCssClasses; } }
        private bool _isVisible = true;

        public FormLineExpression(ITagGenerator<T> tags, ILabelAndFieldLayout layout, Expression<Func<T, object>> expression)
        {
            _tags = tags;
            _layout = layout;
            _expression = expression;

            _layout.LabelTag = tags.LabelFor(expression);
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
            ensureBodyTag();
            alter(_layout.BodyTag);
            return this;
        }

        public FormLineExpression<T> AlterLayout(Action<ILabelAndFieldLayout> alter)
        {
            ensureBodyTag();
            alter(_layout);
            return this;
        }

        public FormLineExpression<T> AlterLayout(Action<ILabelAndFieldLayout, ElementRequest> alter)
        {
            ensureBodyTag();
            var request = _tags.GetRequest(_expression);
            alter(_layout, request);

            return this;
        }

        public FormLineExpression<T> Label(HtmlTag tag)
        {
            _layout.LabelTag = tag;
            return this;
        }

        public FormLineExpression<T> Editable(bool condition)
        {
            if (condition)
            {
                _layout.BodyTag = _tags.InputFor(_expression);    
            }
            else
            {
                _layout.BodyTag = _tags.DisplayFor(_expression);
            }

            return this;
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
            ensureBodyTag();
            _layout.BodyTag.Id(id);
            return this;
        }

        public override string ToString()
        {
            if (!_isVisible) return string.Empty;

            ensureBodyTag();
            AlterLayout(x => GroupByCssClasses.Each(c=>
                                                        {
                                                            x.LabelTag.AddClass(c);
                                                            x.BodyTag.AddClass(c);
                                                        }));
            return _layout.ToString();
        }

        IEnumerable<HtmlTag> ITagSource.AllTags()
        {
            return !_isVisible ? new HtmlTag[0] : _layout.AllTags();
        }

        private void ensureBodyTag()
        {
            if (_layout.BodyTag == null)
            {
                _layout.BodyTag = _tags.DisplayFor(_expression);
            }
        }
    }
}