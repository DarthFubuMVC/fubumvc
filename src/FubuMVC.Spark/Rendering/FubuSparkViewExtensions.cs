using System;

namespace FubuMVC.Spark.Rendering
{
    public static class FubuSparkViewExtensions
    {
        public static IFubuSparkView Modify(this IFubuSparkView view, Action<IFubuSparkView> modify)
        {
            modify(view);
            return view;
        }
    }
}