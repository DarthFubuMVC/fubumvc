using System.Threading.Tasks;
using Marten;

namespace FubuMVC.Marten
{
    public class SessionBoundary : ISessionBoundary
    {
        private readonly object _lock = new object();
        private readonly IMartenSessionLogger _logger;
        private readonly IDocumentStore _store;
        private IDocumentSession _session;

        public SessionBoundary(IDocumentStore store, IMartenSessionLogger logger)
        {
            _store = store;
            _logger = logger;
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
                        _session = _store.OpenSession();
                        _session.Logger = _logger;
                    }
                }
            }

            return _session;
        }

        public async Task SaveChanges()
        {
            if (_session != null)
            {
                await _session.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}