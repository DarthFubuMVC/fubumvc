using System.Collections.Generic;

namespace FubuMVC.Core.Assets.Http
{
    public interface IContentWriter
    {
        void WriteContent(IEnumerable<string> routeParts);
    }
}