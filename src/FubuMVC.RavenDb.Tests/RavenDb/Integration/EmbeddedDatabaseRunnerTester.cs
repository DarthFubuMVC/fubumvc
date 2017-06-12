﻿using System;
using System.Linq;
using FubuMVC.RavenDb.RavenDb;
using Shouldly;
using StructureMap;
using Xunit;

namespace FubuMVC.RavenDb.Tests.RavenDb.Integration
{
    public class EmbeddedDatabaseRunnerTester : IDisposable
    {
        private EmbeddedDatabaseRunner theRunner;
        private Container container;

        public EmbeddedDatabaseRunnerTester()
        {
            theRunner = new EmbeddedDatabaseRunner();
            theRunner.Start();

            container = new Container(x => {
                x.IncludeRegistry<RavenDbRegistry>();

                x.For<RavenDbSettings>().Use(new RavenDbSettings {Url = "http://localhost:8080"});
            });
        }

        [Fact(Skip="Explicit")]
        public void can_connect_remotely()
        {
            var transaction = container.GetInstance<ITransaction>();

            transaction.WithRepository(repo => {
                repo.Update(new Entity1{Name = "Jeremy"});
                repo.Update(new Entity1{Name = "Josh"});
                repo.Update(new Entity1{Name = "Roy"});
            });

            transaction.WithRepository(repo => {
                repo.All<Entity1>().Select(x => x.Name).ToList().OrderBy(x => x)
                    .ShouldHaveTheSameElementsAs("Jeremy", "Josh", "Roy");
            });
        }

        [Fact(Skip="I think this design doesn't work anyway, too many requests")]
        public void clear_persisted_state()
        {
            var transaction = container.GetInstance<ITransaction>();

            transaction.WithRepository(repo =>
            {
                repo.Update(new Entity1 { Name = "Jeremy" });
                repo.Update(new Entity1 { Name = "Josh" });
                repo.Update(new Entity1 { Name = "Roy" });

                repo.Update(new Entity2 { Name = "Jeremy" });
                repo.Update(new Entity2 { Name = "Josh" });
                repo.Update(new Entity2 { Name = "Roy" });

                repo.Update(new Entity3 { Name = "Jeremy" });
                repo.Update(new Entity3 { Name = "Josh" });
                repo.Update(new Entity3 { Name = "Roy" });
            });

            theRunner.ClearPersistedState();

            transaction.WithRepository(repo => {
                repo.All<Entity1>().Any().ShouldBeFalse();
                repo.All<Entity2>().Any().ShouldBeFalse();
                repo.All<Entity3>().Any().ShouldBeFalse();
            });
        }

        public void Dispose()
        {
            theRunner?.Dispose();
            container?.Dispose();
        }

        public class Entity1 : Entity
        {
            public string Name { get; set; }
        }
        public class Entity2 : Entity1{}
        public class Entity3 : Entity1{}
    }
}
