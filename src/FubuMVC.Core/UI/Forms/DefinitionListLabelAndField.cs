using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.Core.UI.Forms
{
    public class DefinitionListLabelAndField : ILabelAndFieldLayout
    {
        private readonly HtmlTag _dt = new HtmlTag("dt");
        private readonly HtmlTag _dd = new HtmlTag("dd");
        private readonly HtmlTag _bodyHolder;

        public DefinitionListLabelAndField()
        {
            _bodyHolder = _dd;
        }

        public HtmlTag DtTag { get { return _dt; } }
        public HtmlTag DdTag { get { return _dd; } }

        public HtmlTag LabelTag 
        { 
            get { return _dt.FirstChild() ?? _dt; }
            set { _dt.ReplaceChildren(value); } 
        }

        public HtmlTag BodyTag 
        { 
            get { return _bodyHolder.FirstChild(); }
            set { _bodyHolder.ReplaceChildren(value); } 
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