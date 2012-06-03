using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Tags;

namespace FubuMVC.Core.UI.Forms
{
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
}