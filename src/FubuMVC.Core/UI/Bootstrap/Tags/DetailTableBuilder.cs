using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.UI.Bootstrap.Tags
{
    public class DetailTableBuilder
    {
        private readonly DetailsTableTag _detailTag;
        private readonly FubuHtmlDocument _document;

        public DetailTableBuilder(FubuHtmlDocument document)
        {
            _document = document;


            _detailTag = new DetailsTableTag();
            _document.Add(_detailTag);
        }

        public DetailsTableTag DetailTag
        {
            get { return _detailTag; }
        }

        public void AddDetail(string label, string text, HtmlEncoding encoding = HtmlEncoding.UseEncoding)
        {
            if (text.IsEmpty()) return;

            _detailTag.AddDetail(label, text);
        }

        public void AddDetailByPartial(string label, object input)
        {
            var content = _document.Partial(input);
            _detailTag.AddDetail(label, content.ToString(), HtmlEncoding.NoEncoding);
        }

        public void AddDetail(string label, IEnumerable<string> values)
        {
            if (!values.Any()) return;

            AddDetail(label, values.Join(", "));
        }
    }
}