using System;
using System.Threading.Tasks;
using Marten;

namespace FubuMVC.Marten
{
    public interface ISessionBoundary : IDisposable
    {
        IDocumentSession Session();

        Task SaveChanges();
    }
}