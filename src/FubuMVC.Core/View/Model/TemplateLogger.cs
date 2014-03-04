using System;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Core.View.Model
{
    public interface ITemplateLogger
    {
        void Log(ITemplateFile template, string format, params object[] args);
        void Log(ITemplateFile template, string text);
    }

    public class NulloLogger : ITemplateLogger
    {
        public void Log(ITemplateFile template, string format, params object[] args) { }
        public void Log(ITemplateFile template, string text) { }
    }

    public class TemplateLogger : ITemplateLogger
    {
        private readonly Action<ITemplateFile, string, object[]> _format;
        private readonly Action<ITemplateFile, string> _trace;

        public TemplateLogger(Action<ITemplateFile, string, object[]> format, Action<ITemplateFile, string> trace)
        {
            _format = format;
            _trace = trace;
        }

        public void Log(ITemplateFile template, string format, params object[] args)
        {
            _format(template, format, args);
        }

        public void Log(ITemplateFile template, string text)
        {
            _trace(template, text);
        }

        public static ITemplateLogger Default()
        {
            return new TemplateLogger(formatTrace, trace);
        }

        private static IPackageLog getPackageLogger(ITemplateFile template)
        {
            if (PackageRegistry.Diagnostics == null) return new PackageLog();

            return PackageRegistry.Diagnostics.LogFor(template);
        }

        private static void formatTrace(ITemplateFile template, string format, object[] args)
        {
            getPackageLogger(template).Trace(format, args);
        }

        private static void trace(ITemplateFile template, string text)
        {
            getPackageLogger(template).Trace(text);
        }
    }
}