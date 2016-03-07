using System;
using Marten;

namespace FubuMVC.Marten
{
    public interface ISessionBoundary : IDisposable
    {
        IDocumentSession Session();
        void WithOpenSession(Action<IDocumentSession> action);
        void SaveChanges();
        void Start();
    }
}