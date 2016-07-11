using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marten;

namespace FubuMVC.Marten
{
    public class SessionBoundary : ISessionBoundary
    {
        private readonly IDocumentStore _store;
        private readonly IMartenSessionLogger _logger;

        private Lazy<IDocumentSession> _session;

        public SessionBoundary(IDocumentStore store, IMartenSessionLogger logger)
        {
            _store = store;
            _logger = logger;

            reset();
        }



        public void Dispose()
        {
            if (_session.IsValueCreated)
            {
                _session.Value.Dispose();   
            }
        }

        public IDocumentSession Session()
        {
            return _session.Value;
        }

        public Task SaveChanges()
        {
            if (_session.IsValueCreated)
            {
                return _session.Value.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }

        public void Start()
        {
            reset();
        }


        private void reset()
        {
            Dispose();

            _session = new Lazy<IDocumentSession>(() =>
            {
                var s = _store.OpenSession();
                s.Logger = _logger;

                return s;
            });
        }
    }
}