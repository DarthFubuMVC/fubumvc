using System;
using Raven.Client;

namespace FubuMVC.RavenDb.RavenDb
{
    public interface ISessionBoundary : IDisposable
    {
        IDocumentSession Session();
        IDocumentSession Session<T>() where T : RavenDbSettings;
        void WithOpenSession(Action<IDocumentSession> action);
        void SaveChanges();
        void Start();
        void MakeReadOnly();
    }
}