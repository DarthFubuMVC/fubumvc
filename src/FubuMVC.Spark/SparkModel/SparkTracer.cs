using System;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkTracer
    {
        void Trace(SparkItem item, string format, params object[] args);
        void Trace(SparkItem item, string text);
    }

    public class NulloTracer : ISparkTracer
    {
        public void Trace(SparkItem item, string format, params object[] args) { }
        public void Trace(SparkItem item, string text) { }
    }

    public class SparkTracer : ISparkTracer
    {
        private readonly Action<SparkItem, string, object[]> _format;
        private readonly Action<SparkItem ,string> _trace;

        public SparkTracer(Action<SparkItem, string, object[]> format, Action<SparkItem, string> trace)
        {
            _format = format;
            _trace = trace;
        }

        public void Trace(SparkItem item, string format, params object [] args)
        {
            _format(item,format, args);
        }

        public void Trace(SparkItem item, string text)
        {
            _trace(item, text);
        }

        public static SparkTracer Default()
        {
            return new SparkTracer(formatTrace, trace);
        }

        private static PackageLog getPackageLogger(SparkItem item)
        {
            return PackageRegistry.Diagnostics.LogFor(item);
        }

        private static void formatTrace(SparkItem item, string format, object[] args)
        {
            getPackageLogger(item).Trace(format, args);
        }

        private static void trace(SparkItem item, string text)
        {
            getPackageLogger(item).Trace(text);
        }
    }
}