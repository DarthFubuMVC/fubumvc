using System;
using System.Threading.Tasks;
using Raven.Client;

namespace FubuMVC.RavenDb.RavenDb
{
    public interface ISessionBoundary : IDisposable
    {
        IDocumentSession Session();
        IDocumentSession Session<T>() where T : RavenDbSettings;
        void WithOpenSession(Action<IDocumentSession> action);
        Task SaveChanges();
        void Start();
        void MakeReadOnly();
    }
}