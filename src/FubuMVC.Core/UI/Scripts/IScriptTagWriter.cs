using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.Core.UI.Scripts
{
    public interface IScriptTagWriter
    {
        IEnumerable<HtmlTag> Write(IEnumerable<string> scripts);
    }
}