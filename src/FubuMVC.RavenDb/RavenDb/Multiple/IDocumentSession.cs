using Raven.Client;

namespace FubuMVC.RavenDb.RavenDb.Multiple
{
    public interface IDocumentSession<TSettings> : IDocumentSession where TSettings : RavenDbSettings
    {
        
    }
}