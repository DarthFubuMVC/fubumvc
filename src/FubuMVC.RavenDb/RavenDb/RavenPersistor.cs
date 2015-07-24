using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Raven.Client;

namespace FubuMVC.RavenDb.RavenDb
{
    public class RavenPersistor : IPersistor
    {
        private readonly Lazy<IDocumentSession> _session;

        public RavenPersistor(ISessionBoundary boundary)
        {
            _session = new Lazy<IDocumentSession>(() =>
            {
                return boundary.Session();
            });
        }

        private IDocumentSession session
        {
            get { return _session.Value; }
        }

        public IQueryable<T> LoadAll<T>() where T : IEntity
        {
            return session.Query<T>();
        }

        public void Persist<T>(T subject) where T : class, IEntity
        {
            session.Store(subject);
        }

        /// <summary>
        ///   Not really worried about perf here because it's
        ///   only going to be used for testing on little bitty
        ///   data sets
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        public void DeleteAll<T>() where T : IEntity
        {
            var subjects = session.Query<T>().ToList();
            subjects.Each(Remove<T>);
        }

        public void Remove<T>(T target) where T : IEntity
        {
            session.Delete(target);
        }

        public T FindSingle<T>(Expression<Func<T, bool>> filter) where T : class, IEntity
        {
            var queriedCopy = session.Query<T>().FirstOrDefault(filter);
            if (queriedCopy == null) return null;

            return Find<T>(queriedCopy.Id);
        }

        public T Find<T>(Guid id) where T : class, IEntity
        {
            return session.Load<T>(id);
        }
    }
}