using Raven.Client;

namespace FubuPersistence.RavenDb.Multiple
{
    public interface IDocumentSession<TSettings> : IDocumentSession where TSettings : RavenDbSettings
    {
        
    }
}