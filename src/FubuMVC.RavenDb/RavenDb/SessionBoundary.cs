using System;
using System.Collections.Generic;
using System.Diagnostics;
using FubuCore.Logging;
using FubuCore.Util;
using FubuMVC.RavenDb.RavenDb.Multiple;
using Raven.Client;
using StructureMap;

namespace FubuMVC.RavenDb.RavenDb
{
    public class SessionBoundary : ISessionBoundary
    {
        private readonly IDocumentStore _store;
        private readonly ILogger _logger;

        private Lazy<IDocumentSession> _session;
        private readonly Cache<Type, IDocumentSession> _otherSessions; 

        public SessionBoundary(IDocumentStore store, IContainer container, ILogger logger)
        {
            _store = store;
            _logger = logger;
            _otherSessions = new Cache<Type, IDocumentSession>(type => {
                var otherStore = container.ForGenericType(typeof (IDocumentStore<>))
                         .WithParameters(type)
                         .GetInstanceAs<IDocumentStore>();

                return otherStore.OpenSession();
            });

            reset();
        }

        private IEnumerable<IDocumentSession> openSessions()
        {
            if (_session != null && _session.IsValueCreated)
            {
                yield return _session.Value;
            }

            foreach (var session in _otherSessions)
            {
                yield return session;
            }
        } 

        public void Dispose()
        {
            WithOpenSession(s =>
            {
                _logger.DebugMessage(() => DisposeRavenSessionMessage.For(s));
                s.Dispose();
            });
            _otherSessions.ClearAll();
        }

        public IDocumentSession Session()
        {
            return _session.Value;
        }

        public IDocumentSession Session<T>() where T : RavenDbSettings
        {
            return _otherSessions[typeof (T)];
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

        public void MakeReadOnly()
        {
            // TODO -- figure out how to make the entire document store / session be readonly 
            // Nothing yet, dadgummit
        }

        private void reset()
        {
            WithOpenSession(s => s.Dispose());
            _session = new Lazy<IDocumentSession>(() =>
            {
                return _store.OpenSession();
            });
        }
    }
}