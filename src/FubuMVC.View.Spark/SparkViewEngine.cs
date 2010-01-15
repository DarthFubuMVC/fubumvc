using System;
using FubuMVC.Core.View;

namespace FubuMVC.View.Spark
{
    public class SparkViewEngine<T> : IViewEngine<T> where T : class
    {
        public void RenderView(ViewPath viewPath, Action<T> configureView)
        {
            throw new NotImplementedException();
        }
    }
}