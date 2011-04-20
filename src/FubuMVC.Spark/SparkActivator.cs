using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkActivator : IActivator
    {
        private readonly IEnumerable<SparkItem> _sparkItems;
        private readonly ISparkViewEngine _engine;
        public SparkActivator(IEnumerable<SparkItem> sparkItems, ISparkViewEngine engine)
        {
            _sparkItems = sparkItems;
            _engine = engine;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _engine.ViewFolder = new SparkItemViewFolder(_sparkItems);
        }
    }
}