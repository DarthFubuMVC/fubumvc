using System;
using System.Collections.Generic;
using System.IO;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IFubuSparkView : IRenderableView, IFubuPage
    {
        Dictionary<string, TextWriter> Content { set; get; }
        Dictionary<string, string> OnceTable { set; get; }
        Dictionary<string, object> Globals { set; get; }
        TextWriter Output { get; set; }
        
        Guid GeneratedViewId { get; }

        ICacheService CacheService { get; set; }
    }
}