using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using Marten;

namespace FubuMVC.Marten
{
    public class SessionBoundary : ISessionBoundary
    {
        private readonly IDocumentStore _store;
        private readonly IMartenSessionLogger _logger;
        private readonly ICurrentChain _chain;
        private readonly object _lock = new object();
        private IDocumentSession _session;

        public SessionBoundary(IDocumentStore store, IMartenSessionLogger logger, ICurrentChain chain)
        {
            _store = store;
            _logger = logger;
            _chain = chain;
        }

        public void Dispose()
        {

        }

        public IDocumentSession Session()
        {
            if (_session == null)
            {
                lock (_lock)
                {
                    if (_session == null)
                    {
                        var node = _chain.OriginatingChain.OfType<TransactionNode>().FirstOrDefault();
                        var isolation = node?.IsolationLevel ?? IsolationLevel.ReadCommitted;

                        _session = _store.OpenSession(isolationLevel:isolation);
                        _session.Logger = _logger;
                    }
                }
            }

            return _session;
        }

        public void SaveChanges()
        {
            _session?.SaveChanges();

        }

    }
}