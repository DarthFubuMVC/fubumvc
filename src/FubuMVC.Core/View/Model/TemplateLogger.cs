using System;
using FubuMVC.Core.Diagnostics.Packaging;

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
        private readonly IBottlingDiagnostics _diagnostics;

        public TemplateLogger(IBottlingDiagnostics diagnostics)
        {
            _diagnostics = diagnostics;
        }

        public void Log(ITemplateFile template, string format, params object[] args)
        {
            formatTrace(template, format, args);
        }

        public void Log(ITemplateFile template, string text)
        {
            trace(template, text);
        }

        public static ITemplateLogger Default(IBottlingDiagnostics diagnostics)
        {
            return new TemplateLogger(diagnostics);
        }

        private IPackageLog getPackageLogger(ITemplateFile template)
        {
            return _diagnostics.LogFor(template);
        }

        private void formatTrace(ITemplateFile template, string format, object[] args)
        {
            getPackageLogger(template).Trace(format, args);
        }

        private void trace(ITemplateFile template, string text)
        {
            getPackageLogger(template).Trace(text);
        }
    }
}