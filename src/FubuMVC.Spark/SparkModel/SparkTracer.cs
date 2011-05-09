using System;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkTracer
    {
        void Trace(ITemplate template, string format, params object[] args);
        void Trace(ITemplate template, string text);
    }

    public class NulloTracer : ISparkTracer
    {
        public void Trace(ITemplate template, string format, params object[] args) { }
        public void Trace(ITemplate template, string text) { }
    }

    public class SparkTracer : ISparkTracer
    {
        private readonly Action<ITemplate, string, object[]> _format;
        private readonly Action<ITemplate, string> _trace;

        public SparkTracer(Action<ITemplate, string, object[]> format, Action<ITemplate, string> trace)
        {
            _format = format;
            _trace = trace;
        }

        public void Trace(ITemplate template, string format, params object[] args)
        {
            _format(template,format, args);
        }

        public void Trace(ITemplate template, string text)
        {
            _trace(template, text);
        }

        public static SparkTracer Default()
        {
            return new SparkTracer(formatTrace, trace);
        }

        private static PackageLog getPackageLogger(ITemplate template)
        {
            return PackageRegistry.Diagnostics.LogFor(template);
        }

        private static void formatTrace(ITemplate template, string format, object[] args)
        {
            getPackageLogger(template).Trace(format, args);
        }

        private static void trace(ITemplate template, string text)
        {
            getPackageLogger(template).Trace(text);
        }
    }
}