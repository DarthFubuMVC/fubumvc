using System;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Razor.RazorModel
{
    public interface IRazorLogger
    {
        void Log(ITemplate template, string format, params object[] args);
        void Log(ITemplate template, string text);
    }

    public class NulloLogger : IRazorLogger
    {
        public void Log(ITemplate template, string format, params object[] args) { }
        public void Log(ITemplate template, string text) { }
    }

    public class RazorLogger : IRazorLogger
    {
        private readonly Action<ITemplate, string, object[]> _format;
        private readonly Action<ITemplate, string> _trace;

        public RazorLogger(Action<ITemplate, string, object[]> format, Action<ITemplate, string> trace)
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

        public static IRazorLogger Default()
        {
            return new RazorLogger(formatTrace, trace);
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