using System;
using System.Linq.Expressions;
using FubuMVC.Core.UI.Tags;
using HtmlTags;

namespace FubuMVC.Core.UI.Forms
{
    public class DefinitionListForm<TModel> where TModel : class
    {
        private HtmlTag _top;
        private SimpleForm<TModel, DefinitionListLabelAndField> _form;

        public DefinitionListForm(ITagGenerator<TModel> generator, TModel model)
        {
            generator.Model = model;
            _form = new SimpleForm<TModel, DefinitionListLabelAndField>(generator);
            
            _top = new HtmlTag("dl");
        }

        public HtmlTag TopTag
        {
            get
            {
                return _top;
            }
        }

        public void Display(Expression<Func<TModel, object>> expression)
        {
            var layout = _form.Display(expression);
            _top.Append(layout);
        }
    }
}