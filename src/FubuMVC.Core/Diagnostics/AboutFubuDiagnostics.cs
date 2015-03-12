namespace FubuMVC.Core.Diagnostics
{
    public class AboutFubuDiagnostics
    {
        private readonly AppReloaded _reloaded;

        public AboutFubuDiagnostics(AppReloaded reloaded)
        {
            _reloaded = reloaded;
        }

        public string get_about()
        {
            return FubuApplicationDescriber.WriteDescription();
        }

        public string get_loaded()
        {
            return _reloaded.Timestamp.ToString();
        }
    }
}