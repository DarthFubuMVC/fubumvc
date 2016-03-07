using System;
using System.Collections.Generic;
using Marten;

namespace FubuMVC.Marten
{
    public class SessionBoundary : ISessionBoundary
    {
        private readonly IDocumentStore _store;

        private Lazy<IDocumentSession> _session;

        public SessionBoundary(IDocumentStore store)
        {
            _store = store;

            reset();
        }

        private IEnumerable<IDocumentSession> openSessions()
        {
            if (_session != null && _session.IsValueCreated)
            {
                yield return _session.Value;
            }
        }

        public void Dispose()
        {
            WithOpenSession(s =>
            {
                s.Dispose();
            });

        }

        public IDocumentSession Session()
        {
            return _session.Value;
        }

        public void WithOpenSession(Action<IDocumentSession> action)
        {
            openSessions().Each(action);
        }

        public void SaveChanges()
        {
            WithOpenSession(s => s.SaveChanges());
        }

        public void Start()
        {
            reset();
        }


        private void reset()
        {
            WithOpenSession(s => s.Dispose());
            _session = new Lazy<IDocumentSession>(() => _store.OpenSession());
        }
    }
}