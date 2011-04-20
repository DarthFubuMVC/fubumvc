using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    // NOTE: Nice thing about activation is that we get IoC - 
    //       So we can move tasks that not necessarily need to happen
    //       during bootstrap to activators.
    public class SparkActivator : IActivator
    {
        private readonly SparkItems _sparkItems;
        private readonly ISparkViewEngine _engine;
        public SparkActivator(SparkItems sparkItems, ISparkViewEngine engine)
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