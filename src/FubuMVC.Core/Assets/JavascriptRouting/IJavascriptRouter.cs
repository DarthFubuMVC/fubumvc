using System.Collections.Generic;

namespace FubuMVC.Core.Assets.JavascriptRouting
{
    public interface IJavascriptRouter
    {
        IEnumerable<JavascriptRoute> Routes();
    }
}