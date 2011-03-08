using System.Collections.Generic;
using HtmlTags;

namespace FubuFastPack.Querying
{
    public interface IFilterTemplateSource
    {
        IEnumerable<HtmlTag> TagsFor(IEnumerable<FilteredProperty> properties);
    }
}