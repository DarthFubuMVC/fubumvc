using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuMVC.RavenDb.RavenDb.Multiple;
using FubuMVC.RavenDb.Reset;
using Raven.Client;
using StructureMap;

namespace FubuMVC.RavenDb.RavenDb
{
    public class RavenPersistenceReset : IPersistenceReset
    {
        private readonly IContainer _container;

        public RavenPersistenceReset(IContainer container)
        {
            _container = container;
        }

        public void ClearPersistedState()
        {
            var ports = new List<int>();

            _container.Model.For<IDocumentStore>().Default.EjectObject();
            _container.Inject(new RavenDbSettings
            {
                RunInMemory = true,
                UseEmbeddedHttpServer = true
            });

            ports.Add(8080);

            // Force the container to spin it up now just in case other things
            // are trying access the store remotely
            var store = _container.GetInstance<IDocumentStore>();
            Debug.WriteLine("Opening up a new in memory RavenDb IDocumentStore at " + store.Url);


            var otherSettingTypes = FindOtherSettingTypes();

            otherSettingTypes.Each(type => {
                var settings = Activator.CreateInstance(type).As<RavenDbSettings>();
                settings.Url = null;
                settings.ConnectionString = null;
                settings.RunInMemory = true;

                if (ports.Contains(settings.Port))
                {
                    var port = ports.Max() + 1;
                    settings.Port = port;
                    ports.Add(port);
                }

                Debug.WriteLine("Starting a new database for {0} at http://localhost:{1}", type.Name, settings.Port);

                settings.UseEmbeddedHttpServer = true;

                _container.Inject(type, settings);

                var documentStoreType = typeof (IDocumentStore<>).MakeGenericType(type);
                _container.Model.For(documentStoreType).Default.EjectObject();

                // Force the container to spin it up now just in case other things
                // are trying access the store remotely
                _container.GetInstance(documentStoreType);
            });

        }

        public IList<Type> FindOtherSettingTypes()
        {
            var otherSettingTypes = _container.Model.PluginTypes.Where(x => x.PluginType.IsConcreteTypeOf<RavenDbSettings>() && x.PluginType != typeof(RavenDbSettings))
                                              .Select(x => x.PluginType).ToList();
            return otherSettingTypes;
        }

        public void CommitAllChanges()
        {
            // no-op for now
        }

        public static void Try()
        {
            
        }
    }
}