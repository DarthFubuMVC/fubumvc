using System;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;

namespace Spark.Web.FubuMVC
{
    public abstract class SparkView : SparkView<object>
    {
        public IResourcePathManager ResourcePathManager { get; set; }
    }

    public abstract class SparkView<TModel> : AbstractSparkView, ISparkView, IFubuView<TModel> where TModel : class
    {
        #region IFubuView<TModel> Members

        public TModel Model { get; private set; }

        public void SetModel(IFubuRequest request)
        {
            Model = request.Get<TModel>();
        }

        #endregion
    }
}