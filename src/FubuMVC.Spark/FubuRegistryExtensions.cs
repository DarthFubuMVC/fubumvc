using System;
using FubuCore;
using FubuMVC.Core;

namespace FubuMVC.Spark
{
    public static class FubuRegistryExtensions
    {
        [Obsolete("This call is completely unnecessary if the FubuMVC.Spark assembly is in the application path")]
        public static void UseSpark(this FubuRegistry fubuRegistry)
        {
            fubuRegistry.UseSpark(s => { });
        }

        [Obsolete("Use FubuRegistry.Import<SparkEngine>(Action<SparkEngine>) instead")]
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