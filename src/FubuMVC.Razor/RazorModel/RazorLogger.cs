using System;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Razor.RazorModel
{
    public interface IRazorLogger
    {
        void Log(IRazorTemplate template, string format, params object[] args);
        void Log(IRazorTemplate template, string text);
    }

    public class NulloLogger : IRazorLogger
    {
        public void Log(IRazorTemplate template, string format, params object[] args) { }
        public void Log(IRazorTemplate template, string text) { }
    }

    public class RazorLogger : IRazorLogger
    {
        private readonly Action<IRazorTemplate, string, object[]> _format;
        private readonly Action<IRazorTemplate, string> _trace;

        public RazorLogger(Action<IRazorTemplate, string, object[]> format, Action<IRazorTemplate, string> trace)
        {
            _format = format;
            _trace = trace;
        }

        public void Log(IRazorTemplate template, string format, params object[] args)
        {
            _format(template, format, args);
        }

        public void Log(IRazorTemplate template, string text)
        {
            _trace(template, text);
        }

        public static IRazorLogger Default()
        {
            return new RazorLogger(formatTrace, trace);
        }

        private static IPackageLog getPackageLogger(IRazorTemplate template)
        {
            return PackageRegistry.Diagnostics.LogFor(template);
        }

        private static void formatTrace(IRazorTemplate template, string format, object[] args)
        {
            getPackageLogger(template).Trace(format, args);
        }

        private static void trace(IRazorTemplate template, string text)
        {
            getPackageLogger(template).Trace(text);
        }
    }
}