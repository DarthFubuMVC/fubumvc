namespace FubuMVC.Core.Registration.Routes
{
    public interface IMakeMyOwnUrl
    {
        string ToUrlPart(string basePattern);
    }
}