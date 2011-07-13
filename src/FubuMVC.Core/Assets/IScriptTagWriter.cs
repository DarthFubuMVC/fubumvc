using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public interface IScriptTagWriter
    {
        IEnumerable<HtmlTag> Write(IEnumerable<string> scripts);
    }
}