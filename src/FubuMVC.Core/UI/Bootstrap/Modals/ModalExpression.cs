using System.Web;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.Bootstrap.Modals
{
    //<a data-toggle="modal" href="#myModal" class="btn btn-primary btn-large">Launch demo modal</a>

    public class ModalExpression : IHtmlString
    {
        private readonly IFubuPage _page;
        private readonly ModalTag _tag;

        public ModalExpression(IFubuPage page, string id)
        {
            _page = page;

            _tag = new ModalTag(id);
        }

        public string ToHtmlString()
        {
            return ToString();
        }

        public ModalExpression Label(string label)
        {
            _tag.Label.Text(label);
            return this;
        }

        public ModalExpression AddCloseButton(string text)
        {
            _tag.AddCloseButton(text);
            return this;
        }

        public ModalExpression AddButton(string text, string id)
        {
            _tag.AddFooterButton(text).Id(id);
            return this;
        }

        public ModalExpression UsePartial(object model, bool withModelBinding = false)
        {
            var text = _page.Partial(model, withModelBinding);
            _tag.Body.Encoded(false).Text(text.ToString());

            return this;
        }

        public ModalExpression UsePartial<T>() where T : class
        {
            var text = _page.Partial<T>();
            _tag.Body.Encoded(false).Text(text.ToString());

            return this;
        }

        public override string ToString()
        {
            return _tag.ToString();
        }
    }
}