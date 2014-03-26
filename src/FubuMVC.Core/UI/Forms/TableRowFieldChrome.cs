using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace FubuMVC.Core.UI.Forms
{
    public class TableRowFieldChrome : IFieldChrome
    {
        public IEnumerable<HtmlTag> AllTags()
        {
            yield return new HtmlTag("tr", tr => {
                tr.Add("td").Append(LabelTag);
                tr.Add("td").Append(BodyTag);
            });
        }

        public HtmlTag LabelTag { get; set; }
        public HtmlTag BodyTag { get; set; }
        public string Render()
        {
            return AllTags().Single().ToString();
        }
    }
}