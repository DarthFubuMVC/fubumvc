using System;
using NHibernate;

namespace FubuFastPack.NHibernate
{
    public interface ITransactionBoundary : IDisposable
    {
        bool IsDisposed { get; }
        void Commit();
        void Rollback();
        void Start();
    }

    public interface INHibernateTransactionBoundary : ITransactionBoundary
    {
        ISession Session { get; }
    }

    /// <summary>
    /// Manages transaction lifecycle. Transactions do not have to be explicitly created
    /// </summary>
    /// <remarks>This should not be used for production application scenarios</remarks>
    public class OnDemandTransactionBoundary : INHibernateTransactionBoundary
    {
        private readonly ISessionFactory _sessionFactory;
        private bool _isInitialized;
        private ISession _session;
        private ITransaction _transaction;

        public OnDemandTransactionBoundary(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ISession Session
        {
            get
            {
                ensure_initialized();
                return _session;
            }
        }

        public bool IsDisposed { get; private set; }

        public void Start()
        {
            _session = _sessionFactory.OpenSession();
            _session.FlushMode = FlushMode.Commit;
            _transaction = _session.BeginTransaction();
            _isInitialized = true;
        }

        public void Commit()
        {
            should_not_be_disposed();
            if (!_isInitialized) return;
            _transaction.Commit();
        }

        public void Rollback()
        {
            should_not_be_disposed();
            if (!_isInitialized) return;
            _transaction.Rollback();

            _transaction = _session.BeginTransaction();
        }

        public void Dispose()
        {
            IsDisposed = true;
            if (_transaction != null) _transaction.Dispose();
            if (_session != null) _session.Dispose();
        }

        private void should_not_be_disposed()
        {
            if (!IsDisposed) return;
            throw new ObjectDisposedException(GetType().Name);
        }

        private void ensure_initialized()
        {
            if (!_isInitialized)
            {
                Start();
            }
        }
    }

    /// <summary>
    /// Manages transaction lifecycle. Transactions must be explicitly started
    /// </summary>
    public class ExplicitTransactionBoundary : INHibernateTransactionBoundary
    {
        private readonly ISessionFactory _factory;
        private bool _isInitialized;
        private ISession _session;
        private ITransaction _transaction;

        public ExplicitTransactionBoundary(ISessionFactory factory)
        {
            _factory = factory;
        }

        public ISession Session
        {
            get
            {
                ensure_initialized();
                return _session;
            }
        }

        public bool IsDisposed { get; private set; }

        public void Start()
        {
            _session = _factory.OpenSession();
            _session.FlushMode = FlushMode.Commit;

            _transaction = _session.BeginTransaction();
            _isInitialized = true;
        }

        public void Commit()
        {
            should_not_be_disposed();
            ensure_initialized();
            _transaction.Commit();
        }

        public void Rollback()
        {
            should_not_be_disposed();
            ensure_initialized();
            _transaction.Rollback();

            _transaction = _session.BeginTransaction();
        }

        public void Dispose()
        {
            IsDisposed = true;
            cleanupTransaction();
            if (_session != null) _session.Dispose();
        }

        private void cleanupTransaction()
        {
            if (_transaction != null) _transaction.Dispose();
        }

        private void should_not_be_disposed()
        {
            if (!IsDisposed) return;
            throw new ObjectDisposedException(GetType().Name);
        }

        private void ensure_initialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException(
                    "An attempt was made to access the database session outside of a transaction. Please make sure all access is made within an initialized transaction boundary.");
            }
        }
    }
}