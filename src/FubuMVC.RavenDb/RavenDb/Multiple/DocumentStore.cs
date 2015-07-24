using System;
using System.Collections.Specialized;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Changes;
using Raven.Client.Connection;
using Raven.Client.Connection.Async;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace FubuMVC.RavenDb.RavenDb.Multiple
{
    public interface IDocumentStore<T> : IDocumentStore where T : RavenDbSettings
    {
        
    }

    public class DocumentStore<T> : IDocumentStore<T> where T : RavenDbSettings
    {
        private readonly IDocumentStore _inner;

        public DocumentStore(IDocumentStore inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public bool WasDisposed
        {
            get { return _inner.WasDisposed; }
        }

        public event EventHandler AfterDispose
        {
            add { _inner.AfterDispose += value; }
            remove { _inner.AfterDispose -= value; }
        }

        public IDatabaseChanges Changes(string database = null)
        {
            return _inner.Changes(database);
        }

        public IDisposable AggressivelyCacheFor(TimeSpan cacheDuration)
        {
            return _inner.AggressivelyCacheFor(cacheDuration);
        }

        public IDisposable AggressivelyCache()
        {
            return _inner.AggressivelyCache();
        }

        public IDisposable DisableAggressiveCaching()
        {
            return _inner.DisableAggressiveCaching();
        }

        public IDisposable SetRequestsTimeoutFor(TimeSpan timeout)
        {
            return _inner.SetRequestsTimeoutFor(timeout);
        }

        public IDocumentStore Initialize()
        {
            throw new NotSupportedException("This is a wrapped IDocumentStore that will already be initialized");
        }

        public IAsyncDocumentSession OpenAsyncSession()
        {
            return _inner.OpenAsyncSession();
        }

        public IAsyncDocumentSession OpenAsyncSession(string database)
        {
            return _inner.OpenAsyncSession(database);
        }

        public IDocumentSession OpenSession()
        {
            return _inner.OpenSession();
        }

        public IDocumentSession OpenSession(string database)
        {
            return _inner.OpenSession(database);
        }

        public IDocumentSession OpenSession(OpenSessionOptions sessionOptions)
        {
            return _inner.OpenSession(sessionOptions);
        }

        public void ExecuteIndex(AbstractIndexCreationTask indexCreationTask)
        {
            _inner.ExecuteIndex(indexCreationTask);
        }

        public void ExecuteTransformer(AbstractTransformerCreationTask transformerCreationTask)
        {
            _inner.ExecuteTransformer(transformerCreationTask);
        }

        public Etag GetLastWrittenEtag()
        {
            return _inner.GetLastWrittenEtag();
        }

        public BulkInsertOperation BulkInsert(string database = null, BulkInsertOptions options = null)
        {
            return _inner.BulkInsert(database, options);
        }

        public NameValueCollection SharedOperationsHeaders { get { return _inner.SharedOperationsHeaders; } }
        public HttpJsonRequestFactory JsonRequestFactory { get { return _inner.JsonRequestFactory; } }
        public string Identifier
        {
            get { return typeof (T).Name; }
            set
            {
                // no-op;
            }
        }
        public IAsyncDatabaseCommands AsyncDatabaseCommands { get { return _inner.AsyncDatabaseCommands; } }
        public IDatabaseCommands DatabaseCommands { get { return _inner.DatabaseCommands; } }
        public DocumentConvention Conventions { get { return _inner.Conventions; } }
        public string Url { get { return _inner.Url; } }
    }
}