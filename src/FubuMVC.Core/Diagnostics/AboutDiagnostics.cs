namespace FubuMVC.Core.Diagnostics
{
    public class AboutDiagnostics
    {
        private readonly AppReloaded _reloaded;

        public AboutDiagnostics(AppReloaded reloaded)
        {
            _reloaded = reloaded;
        }

        public string get__about()
        {
            return FubuApplicationDescriber.WriteDescription();
        }

        public string get__loaded()
        {
            return _reloaded.Timestamp.ToString();
        }
    }
}