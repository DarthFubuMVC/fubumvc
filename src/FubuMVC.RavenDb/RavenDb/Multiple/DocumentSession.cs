using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace FubuMVC.RavenDb.RavenDb.Multiple
{
    public class DocumentSession<TSettings> : IDocumentSession<TSettings> where TSettings : RavenDbSettings
    {
        private readonly IDocumentSession _inner;

        public DocumentSession(ISessionBoundary boundary)
        {
            _inner = boundary.Session<TSettings>();
        }


        public void Dispose()
        {
            _inner.Dispose();
        }

        public void Delete<T>(T entity)
        {
            _inner.Delete(entity);
        }

        public T Load<T>(string id)
        {
            return _inner.Load<T>(id);
        }

        public T[] Load<T>(params string[] ids)
        {
            return _inner.Load<T>(ids);
        }

        public T[] Load<T>(IEnumerable<string> ids)
        {
            return _inner.Load<T>(ids);
        }

        public T Load<T>(ValueType id)
        {
            return _inner.Load<T>(id);
        }

        public T[] Load<T>(params ValueType[] ids)
        {
            return _inner.Load<T>(ids);
        }

        public T[] Load<T>(IEnumerable<ValueType> ids)
        {
            return _inner.Load<T>(ids);
        }

        public IRavenQueryable<T> Query<T>(string indexName, bool isMapReduce = false)
        {
            return _inner.Query<T>(indexName, isMapReduce);
        }

        public IRavenQueryable<T> Query<T>()
        {
            return _inner.Query<T>();
        }

        public IRavenQueryable<T> Query<T, TIndexCreator>() where TIndexCreator : AbstractIndexCreationTask, new()
        {
            return _inner.Query<T, TIndexCreator>();
        }

        public ILoaderWithInclude<object> Include(string path)
        {
            return _inner.Include(path);
        }

        public ILoaderWithInclude<T> Include<T>(Expression<Func<T, object>> path)
        {
            return _inner.Include<T>(path);
        }

        public ILoaderWithInclude<T> Include<T, TInclude>(Expression<Func<T, object>> path)
        {
            return _inner.Include<T, TInclude>(path);
        }

        public TResult Load<TTransformer, TResult>(string id) where TTransformer : AbstractTransformerCreationTask, new()
        {
            return _inner.Load<TTransformer, TResult>(id);
        }

        public TResult Load<TTransformer, TResult>(string id, Action<ILoadConfiguration> configure) where TTransformer : AbstractTransformerCreationTask, new()
        {
            return _inner.Load<TTransformer, TResult>(id, configure);
        }

        public TResult[] Load<TTransformer, TResult>(params string[] ids) where TTransformer : AbstractTransformerCreationTask, new()
        {
            return _inner.Load<TTransformer, TResult>(ids);
        }

        public TResult[] Load<TTransformer, TResult>(IEnumerable<string> ids, Action<ILoadConfiguration> configure) where TTransformer : AbstractTransformerCreationTask, new()
        {
            return _inner.Load<TTransformer, TResult>(ids, configure);
        }

        public void SaveChanges()
        {
            _inner.SaveChanges();
        }

        public void Store(object entity, Etag etag)
        {
            _inner.Store(entity, etag);
        }

        public void Store(object entity, Etag etag, string id)
        {
            _inner.Store(entity, etag, id);
        }

        public void Store(object entity)
        {
            _inner.Store(entity);
        }

        public void Store(object entity, string id)
        {
            _inner.Store(entity, id);
        }

        public ISyncAdvancedSessionOperation Advanced { get { return _inner.Advanced; } }
    }
}