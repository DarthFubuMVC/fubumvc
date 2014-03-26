using FubuCore;

namespace FubuMVC.Diagnostics
{
    public class DiagnosticsException : FubuException
    {
        public DiagnosticsException(int errorCode, string template, params string[] substitutions)
            : base(errorCode, template, substitutions)
        {
        }
    }
}