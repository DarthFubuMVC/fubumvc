using System;
using System.Threading;
using FubuMVC.RavenDb.RavenDb;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database.Extensions;
using Shouldly;
using Process = System.Diagnostics.Process;

namespace FubuMVC.RavenDb.Tests.RavenDb
{
    [TestFixture]
    public class RavenDbSettingsTester
    {
        [Test]
        public void builds_in_memory()
        {
            var settings = new RavenDbSettings {RunInMemory = true};
            using (var store = settings.Create())
            {
                store.ShouldBeOfType<EmbeddableDocumentStore>().RunInMemory.ShouldBeTrue();
            }
        }

        [Test]
        public void uses_the_port_number_if_it_is_non_zero()
        {
            var settings = new RavenDbSettings { RunInMemory = true, Port = 8081};

            using (var store = settings.Create())
            {
                store.ShouldBeOfType<EmbeddableDocumentStore>()
                    .Configuration.Port.ShouldBe(8081);
            }
        }

        [Test]
        public void uses_default_port_number_if_none()
        {
            var settings = new RavenDbSettings { RunInMemory = true };

            using (var store = settings.Create())
            {
                // 8080 is RavenDb's default port number
                store.ShouldBeOfType<EmbeddableDocumentStore>()
                    .Configuration.Port.ShouldBe(8080);
            }
        }


        [Test]
        public void build_empty_does_not_throw_but_connects_to_the_parallel_data_folder()
        {
            using( var store = new RavenDbSettings().Create())
            {
                store.ShouldBeOfType<EmbeddableDocumentStore>()
                     .DataDirectory.ShouldEndWith("data");
            }

        }

        [Test]
        public void in_memory_is_wait_for_it_in_memory()
        {
            var settings = RavenDbSettings.InMemory();

            var store = settings.Create();
            store.ShouldBeOfType<EmbeddableDocumentStore>().RunInMemory.ShouldBeTrue();

            store.Dispose();
        }

        [Test]
        public void build_in_memory()
        {
            var store = createStore<EmbeddableDocumentStore>(x => x.RunInMemory = true);
            store.RunInMemory.ShouldBeTrue();
            store.UseEmbeddedHttpServer.ShouldBeFalse();

            store.Dispose();
        }

        [Test]
        public void build_using_embedded_http_server_in_memory()
        {
            var store = createStore<EmbeddableDocumentStore>(x =>
            {
                x.RunInMemory = true;
                x.UseEmbeddedHttpServer = true;
            });
            store.RunInMemory.ShouldBeTrue();
            store.UseEmbeddedHttpServer.ShouldBeTrue();

            store.Dispose();
        }

        [Test]
        public void build_using_embedded_http_server_with_data_directory()
        {
            var store = createStore<EmbeddableDocumentStore>(x =>
            {
                x.DataDirectory = "data".ToFullPath();
                x.UseEmbeddedHttpServer = true;
            });
            store.DataDirectory.ShouldBe("data".ToFullPath());
            store.UseEmbeddedHttpServer.ShouldBeTrue();

            store.Dispose();
        }

        [Test]
        public void build_with_data_directory()
        {
            var store = createStore<EmbeddableDocumentStore>(x => x.DataDirectory = "data".ToFullPath());
            store.DataDirectory.ShouldBe("data".ToFullPath());
            store.UseEmbeddedHttpServer.ShouldBeFalse();
            store.Dispose();
        }

        [Test]
        public void build_with_url()
        {
            var store = createStore<DocumentStore>(x => x.Url = "http://somewhere:8080");
            store.Url.ShouldBe("http://somewhere:8080");

            store.Dispose();
        }

        [Test]
        public void is_empty()
        {
            new RavenDbSettings().IsEmpty().ShouldBeTrue();

            new RavenDbSettings
            {
                RunInMemory = true
            }.IsEmpty().ShouldBeFalse();

            new RavenDbSettings
            {
                DataDirectory = "data"
            }.IsEmpty().ShouldBeFalse();

            new RavenDbSettings
            {
                Url = "http://server.com"
            }.IsEmpty().ShouldBeFalse();
        }

        private T createStore<T>(Action<RavenDbSettings> setup) where T : IDocumentStore
        {
            var settings = new RavenDbSettings();
            if (setup != null) setup(settings);
            using (var documentStore = settings.Create())
            {
                return documentStore.ShouldBeOfType<T>();
            }
        }

        [Test, Explicit]
        public void load_a_store_with_explicit_port_see_the_hosted_url()
        {
            var settings = new RavenDbSettings {RunInMemory = true, Port = 8082, UseEmbeddedHttpServer = true};
            using (var store = settings.Create())
            {
                store.Initialize();

                using (var session = store.OpenSession())
                {
                    session.Store(new House
                    {
                        Whose = "Mine",
                        Id = Guid.NewGuid()
                    });

                    session.SaveChanges();
                }


                Process.Start("http://localhost:8082");

                Thread.Sleep(30000);


            }
        }
    }

    public class House : Entity
    {
        public string Whose { get; set; }
    }
}