namespace FubuMVC.Core.Diagnostics
{
    public class AboutEndpoint
    {
        public string get__about()
        {
            return FubuApplicationDescriber.WriteDescription();
        }
    }
}