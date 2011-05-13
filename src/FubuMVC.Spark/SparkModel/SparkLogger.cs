using System;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkLogger
    {
        void Log(ITemplate template, string format, params object[] args);
        void Log(ITemplate template, string text);
    }

    public class NulloLogger : ISparkLogger
    {
        public void Log(ITemplate template, string format, params object[] args) { }
        public void Log(ITemplate template, string text) { }
    }

    public class SparkLogger : ISparkLogger
    {
        private readonly Action<ITemplate, string, object[]> _format;
        private readonly Action<ITemplate, string> _trace;

        public SparkLogger(Action<ITemplate, string, object[]> format, Action<ITemplate, string> trace)
        {
            _format = format;
            _trace = trace;
        }

        public void Log(ITemplate template, string format, params object[] args)
        {
            _format(template, format, args);
        }

        public void Log(ITemplate template, string text)
        {
            _trace(template, text);
        }

        public static ISparkLogger Default()
        {
            return new SparkLogger(formatTrace, trace);
        }

        private static IPackageLog getPackageLogger(ITemplate template)
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