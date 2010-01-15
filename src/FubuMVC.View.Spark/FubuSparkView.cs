using System;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using Spark;

namespace FubuMVC.View.Spark
{
    public interface IFubuSparkView : ISparkView {}
    public abstract class FubuSparkView : FubuSparkView<object>
    {
    }

    public abstract class FubuSparkView<T> : AbstractSparkView, IFubuSparkView, IFubuView<T> where T : class
    {
        public T Model { get; private set; }
        public void SetModel(IFubuRequest request)
        {
            Model = request.Get<T>();
        }
    }

}
