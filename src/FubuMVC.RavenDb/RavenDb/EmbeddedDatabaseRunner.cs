using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.RavenDb.Reset;
using Raven.Client;
using Raven.Database.Server;

namespace FubuMVC.RavenDb.RavenDb
{
    public class EmbeddedDatabaseRunner : IDisposable, IPersistenceReset
    {
        private IDocumentStore _store;

        public IDocumentStore Store
        {
            get { return _store; }
        }

        public void Dispose()
        {
            _store.Dispose();
        }

        public void Start()
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);

            RavenDbSettings settings = RavenDbSettings.InMemory();
            settings.UseEmbeddedHttpServer = true;

            _store = settings.Create();
            _store.Initialize();
        }

        public RavenDbSettings SettingsToConnect()
        {
            return new RavenDbSettings{Url = "http://localhost:8080"};
        }

        public void ClearPersistedState()
        {
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                                               .SelectMany(a =>
                                               {
                                                   try
                                                   {
                                                       return a.GetExportedTypes();
                                                   }
                                                   catch (Exception)
                                                   {
                                                       return new Type[0];
                                                   }
                                               }).Where(x => x.IsConcreteTypeOf<IEntity>());


            using (IDocumentSession session = _store.OpenSession())
            {
                types.Each(type =>
                {
                    typeof(Deleter<>).CloseAndBuildAs<IDeleter>(type)
                                      .Delete(session);
                });

                session.SaveChanges();
            }
        }

        public class Deleter<T> : IDeleter
        {
            public void Delete(IDocumentSession session)
            {
                T[] all = session.Query<T>().ToArray();
                all.Each(x => session.Delete(x));
            }
        }

        public interface IDeleter
        {
            void Delete(IDocumentSession session);
        }


        public void CommitAllChanges()
        {
            // do nothing
        }
    }
}