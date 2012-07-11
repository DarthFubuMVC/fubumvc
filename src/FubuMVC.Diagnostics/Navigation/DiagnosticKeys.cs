using FubuLocalization;

namespace FubuMVC.Diagnostics.Navigation
{
    public class DiagnosticKeys : StringToken
    {
        public static readonly DiagnosticKeys Main = new DiagnosticKeys("Main");
        public static readonly DiagnosticKeys Dashboard = new DiagnosticKeys("Dashboard");
        public static readonly DiagnosticKeys HtmlConventions = new DiagnosticKeys("Html Conventions");
        public static readonly DiagnosticKeys ApplicationStartup = new DiagnosticKeys("Application Startup");
        public static readonly DiagnosticKeys Requests = new DiagnosticKeys("Requests");
        public static readonly DiagnosticKeys Routes = new DiagnosticKeys("Routes");

        public DiagnosticKeys(string defaultValue) : base(null, defaultValue, namespaceByType: true)
        {
        }

        public bool Equals(DiagnosticKeys other)
        {
            return other.Key.Equals(Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as DiagnosticKeys);
        }

        public override int GetHashCode()
        {
            return ("DiagnosticKeys:" + Key).GetHashCode();
        }
    }
}