using System;
using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.UI.Forms
{
    public class DefinitionListLabelAndField : ILabelAndFieldLayout
    {
        private readonly HtmlTag _dt = new HtmlTag("dt");
        private readonly HtmlTag _dl = new HtmlTag("dl");
        private HtmlTag _bodyHolder;

        public DefinitionListLabelAndField()
        {
            _bodyHolder = _dl;
        }

        public HtmlTag LabelTag 
        { 
            get { return _dt.FirstChild(); }
            set { _dt.ReplaceChildren(value); } 
        }

        public HtmlTag BodyTag 
        { 
            get { return _bodyHolder.FirstChild(); }
            set { _bodyHolder.ReplaceChildren(value); } 
        }

        public void WrapBody(HtmlTag tag)
        {
            tag.ReplaceChildren(BodyTag);
            _bodyHolder.ReplaceChildren(tag);
            _bodyHolder = tag;
        }

        public HtmlTag WrapBody(string tagName)
        {
            var tag = new HtmlTag(tagName);

            WrapBody(tag);

            return tag;
        }

        public void SetLabelText(string text)
        {
            _dt.Children.Clear();
            _dt.Text(text);
        }

        public IEnumerable<HtmlTag> AllTags()
        {
            yield return _dt;
            yield return _bodyHolder;
        }

        public override string ToString()
        {
            return string.Format("{0}\n{1}", _dt, _bodyHolder);
        }
    }
}