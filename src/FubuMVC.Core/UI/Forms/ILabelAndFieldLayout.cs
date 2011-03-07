using System;
using System.Linq.Expressions;
using FubuMVC.Core.UI.Tags;
using HtmlTags;
using FubuCore.Reflection;

namespace FubuMVC.Core.UI.Forms
{
    public interface ILabelAndFieldLayout : ITagSource
    {
        HtmlTag LabelTag { get; set; }
        HtmlTag BodyTag { get; set; }
    }

    // TODO -- Jeremy to unit test
    public class SimpleForm<TModel, TLayout> where TLayout : ILabelAndFieldLayout, new() where TModel : class
    {
        private readonly ITagGenerator<TModel> _generator;

        public SimpleForm(ITagGenerator<TModel> generator)
        {
            _generator = generator;
        }

        public TLayout Display(Expression<Func<TModel, object>> expression)
        {
            var layout = new TLayout();
            var accessor = expression.ToAccessor();
            var request = _generator.GetRequest(accessor);

            layout.LabelTag = _generator.LabelFor(request);
            layout.BodyTag = _generator.DisplayFor(request);

            return layout;
        }


    }

    // TODO -- Jeremy to unit test
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