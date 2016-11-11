using System;
using Marten;

namespace FubuMVC.Marten
{
    public interface ISessionBoundary : IDisposable
    {
        IDocumentSession Session();
        void SaveChanges();
    }
}