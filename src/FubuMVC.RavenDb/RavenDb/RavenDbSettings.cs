using System;
using FubuCore;
using FubuCore.Binding;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace FubuMVC.RavenDb.RavenDb
{
    public class RavenDbSettings
    {
        public string DataDirectory { get; set; }
        public bool RunInMemory { get; set; }
        public string Url { get; set; }
        public bool UseEmbeddedHttpServer { get; set; }
        public int Port { get; set; }

        public static RavenDbSettings InMemory()
        {
            return new RavenDbSettings
            {
                ConnectionString = null,
                DataDirectory = null, 
                RunInMemory = true
            };
        }

        [ConnectionString]
        public string ConnectionString { get; set; }

        public bool IsEmpty()
        {
            return !RunInMemory && DataDirectory.IsEmpty() && Url.IsEmpty();
        }

        public IDocumentStore Create()
        {
            if (IsEmpty())
            {
                var defaultDataDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.ParentDirectory().AppendPath("data");
                return new RavenDbSettings {DataDirectory = defaultDataDirectory}.Create();
            }

            if (Url.IsNotEmpty())
            {
                var store = new DocumentStore
                {
                    Url = Url
                };

                if (ConnectionString.IsNotEmpty())
                {
                    store.ParseConnectionString(ConnectionString);
                }

                return store;
            }

            if (DataDirectory.IsNotEmpty())
            {
                return new EmbeddableDocumentStore
                {
                    DataDirectory = DataDirectory,
                    UseEmbeddedHttpServer = UseEmbeddedHttpServer
                };
            }

            if (RunInMemory)
            {
                var store = new EmbeddableDocumentStore
                {
                    RunInMemory = true,
                    UseEmbeddedHttpServer = UseEmbeddedHttpServer
                };

                if (Url.IsNotEmpty())
                {
                    store.Url = Url;
                }

                if (Port > 0)
                {
                    store.Configuration.Port = Port;
                }

                return store;
            }

            throw new ArgumentOutOfRangeException("settings");
        }
    }
}