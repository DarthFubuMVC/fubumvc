using System;
using FubuCore;
using FubuMVC.Core;

namespace FubuMVC.Spark
{
    public static class FubuRegistryExtensions
    {
        public static void UseSpark(this FubuRegistry fubuRegistry)
        {
            fubuRegistry.UseSpark(s => { });
        }

        public static void UseSpark(this FubuRegistry fubuRegistry, Action<SparkEngine> configure)
        {
            var spark = new SparkEngine();
            configure(spark);
            spark
                .As<IFubuRegistryExtension>()
                .Configure(fubuRegistry);
        }
    }
}