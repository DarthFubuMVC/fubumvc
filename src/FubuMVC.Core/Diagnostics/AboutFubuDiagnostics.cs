namespace FubuMVC.Core.Diagnostics
{
    public class AboutFubuDiagnostics
    {
        private readonly AppReloaded _reloaded;

        public AboutFubuDiagnostics(AppReloaded reloaded)
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