using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using Marten;
using Xunit;
using Shouldly;

namespace FubuMVC.Marten.Tests
{

    public class MartenApp : FubuRegistry
    {
        public MartenApp()
        {
            AlterSettings<StoreOptions>(_ =>
            {
                _.Connection(ConnectionSource.GetConnectionString());
                _.AutoCreateSchemaObjects = AutoCreate.All;
            });

            //Policies.Local.Add<TransactionalBehaviorPolicy>();
        }
    }

    
    public class wires_up_marten_into_fubumvc_app_Tests
    {
        public void set_up_connection_string()
        {
            ConnectionSource.SetConnectionString("Host=localhost;Username=postgres;Password=jasper;Database=marten_test");
        }

        [Fact]
        public void should_wire_up_document_store_into_application()
        {
            using (var runtime = FubuRuntime.For<MartenApp>())
            {
                var store = runtime.Get<IDocumentStore>();
                store.Advanced.Clean.CompletelyRemoveAll();

                using (var session = store.LightweightSession())
                {
                    session.Store(new User {Id = "Jeremy"}, new User {Id = "Corey"}, new User {Id = "Jens"});
                    session.SaveChanges();
                    session.Query<User>().Count().ShouldBe(3);
                }
            }
        }

        [Fact]
        public void command_logging_in_prod_mode()
        {

            using (var runtime = FubuRuntime.For<MartenApp>())
            {
                runtime.Get<IMartenSessionLogger>().ShouldBeOfType<NulloMartenLogger>();

                runtime.Get<IDocumentSession>().Logger.ShouldNotBeOfType<CommandRecordingLogger>();
                runtime.Get<IQuerySession>().Logger.ShouldNotBeOfType<CommandRecordingLogger>();


            }
        }

        [Fact]
        public void command_logging_in_dev_mode()
        {
            var app = new MartenApp {Mode = "Development"};

            using (var runtime = app.ToRuntime())
            {
                runtime.Get<IMartenSessionLogger>().ShouldBeOfType<CommandRecordingLogger>();

                runtime.Get<IDocumentSession>().Logger.ShouldBeOfType<CommandRecordingLogger>();
                runtime.Get<IQuerySession>().Logger.ShouldBeOfType<CommandRecordingLogger>();


            }
        }

        [Fact]
        public void querysession_is_wired_up()
        {
            using (var runtime = FubuRuntime.For<MartenApp>())
            {
                var store = runtime.Get<IDocumentStore>();

                store.Advanced.Clean.CompletelyRemoveAll();

                using (var session = store.LightweightSession())
                {
                    session.Store(new User { Id = "Jeremy" }, new User { Id = "Corey" }, new User { Id = "Jens" });
                    session.SaveChanges();
                }

                runtime.Scenario(_ =>
                {
                    _.Get.Url("");
                    _.ContentShouldBe("Corey, Jens, Jeremy");
                });
            }
        }

        [Fact]
        public void documentsession_is_wired_up()
        {
            using (var runtime = FubuRuntime.For<MartenApp>())
            {
                var store = runtime.Get<IDocumentStore>();

                store.Advanced.Clean.CompletelyRemoveAll();

                using (var session = store.LightweightSession())
                {
                    session.Store(new User { Id = "Jeremy" }, new User { Id = "Corey" }, new User { Id = "Jens" });
                    session.SaveChanges();
                }

                runtime.Scenario(_ =>
                {
                    _.Post.Json(new User {Id = "Tim"}).Accepts("text/plain");
                    _.ContentShouldBe("Corey, Jens, Jeremy, Tim");
                });
            }
        }

        [Fact]
        public void use_the_transactional_behavior_policy()
        {
            var registry = new MartenApp();
            registry.Policies.Local.Add<TransactionalBehaviorPolicy>();

            using (var runtime = registry.ToRuntime())
            {
                var store = runtime.Get<IDocumentStore>();

                store.Advanced.Clean.CompletelyRemoveAll();

                using (var session = store.LightweightSession())
                {
                    session.Store(new User { Id = "Jeremy" }, new User { Id = "Corey" }, new User { Id = "Jens" });
                    session.SaveChanges();
                }

                runtime.Scenario(_ =>
                {
                    _.Put.Json(new User {Id = "Nieve"}).Accepts("text/plain");
                });

                using (var session = store.LightweightSession())
                {
                    session.Load<User>("Nieve").ShouldNotBeNull();
                }
            }
        }

        [Fact]
        public void use_transaction()
        {
            using (var runtime = FubuRuntime.For<MartenApp>())
            {
                runtime.Get<IDocumentStore>().Advanced.Clean.CompletelyRemoveAll();

                var tx = runtime.Get<ITransaction>();
                tx.Execute<IDocumentSession>(session => session.Store(new User {Id = "Hank"}));

                using (var session = runtime.Get<IQuerySession>())
                {
                    session.Load<User>("Hank").ShouldNotBeNull();
                }
            }
        }

        [Fact]
        public void use_complete_reset()
        {
            using (var runtime = FubuRuntime.For<MartenApp>())
            {
                var store = runtime.Get<IDocumentStore>();
                store.Advanced.Clean.CompletelyRemoveAll();

                using (var session = store.LightweightSession())
                {
                    session.Store(new User { Id = "Jeremy" }, new User { Id = "Corey" }, new User { Id = "Jens" });
                    session.SaveChanges();
                    session.Query<User>().Count().ShouldBe(3);
                }

                runtime.Get<ICompleteReset>().ResetState();

                using (var session = store.QuerySession())
                {
                    session.Query<User>().Any().ShouldBeFalse();
                }
            }
        }
    }

    public class HomeEndpoint
    {
        private readonly IQuerySession _session;

        public HomeEndpoint(IQuerySession session)
        {
            _session = session;
        }

        public string Index()
        {
            return _session.Query<User>().OrderBy(x => x.Id).ToArray().Select(x => x.Id).Join(", ");
        }


    }

    public class UpdateEndpoint
    {
        private readonly IDocumentSession _session;
        private readonly HomeEndpoint _home;

        public UpdateEndpoint(IDocumentSession session, HomeEndpoint home)
        {
            _session = session;
            _home = home;
        }

        public string post_user(User user)
        {
            _session.Store(user);
            _session.SaveChanges();

            return _home.Index();
        }

        public string put_user(User user)
        {
            _session.Store(user);
            return "okay";
        }
    }

    public class User
    {
        public string Id;
    }
}