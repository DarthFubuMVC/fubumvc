using System;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuMVC.RavenDb.RavenDb;
using FubuMVC.RavenDb.RavenDb.Multiple;
using FubuMVC.RavenDb.Reset;
using FubuMVC.RavenDb.Tests.RavenDb.Integration;
using Shouldly;
using StructureMap;
using Xunit;
using Process = System.Diagnostics.Process;

namespace FubuMVC.RavenDb.Tests.RavenDb
{
    public class RavenPersistenceResetTester : IDisposable
    {
        private Container container;
        private RavenUnitOfWork theUnitOfWork;
        private IPersistenceReset theReset;

        public RavenPersistenceResetTester()
        {
            container = new Container(new RavenDbRegistry());
            container.Inject(new RavenDbSettings { RunInMemory = true });

            theReset = container.GetInstance<IPersistenceReset>();
            theUnitOfWork = new RavenUnitOfWork(container);
        }

        [Fact(Skip ="Manual only testing")]
        public void can_access_the_new_store_by_url()
        {
            theReset.ClearPersistedState();
            Process.Start("http://localhost:8080");
            Thread.Sleep(60000);
        }

        [Fact]
        public void can_find_other_setting_types()
        {
            container.Inject(new SecondDbSettings());
            container.Inject(new ThirdDbSettings());
            container.Inject(new FourthDbSettings());

            theReset.As<RavenPersistenceReset>()
                .FindOtherSettingTypes()
                .OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof(FourthDbSettings), typeof(SecondDbSettings), typeof(ThirdDbSettings));


        }

        [Fact]
        public void reset_wipes_the_slate_clean()
        {
            var repo = theUnitOfWork.Start();

            repo.Update(new User());
            repo.Update(new User());
            repo.Update(new User());
            repo.Update(new OtherEntity());
            repo.Update(new OtherEntity());
            repo.Update(new ThirdEntity());

            theUnitOfWork.Commit();

            theReset.ClearPersistedState();

            repo = theUnitOfWork.Start();

            repo.All<User>().Count().ShouldBe(0);
            repo.All<OtherEntity>().Count().ShouldBe(0);
            repo.All<ThirdEntity>().Count().ShouldBe(0);
        }

        public void Dispose()
        {
            container?.Dispose();
        }
    }

    public class when_clearing_persisted_state_with_multiple_settings
    {
        [Fact(Skip = "some cleanup problems here.")]
        public void ejects_the_store_for_each_and_uses_in_memory_for_each_additional_type_of_setting()
        {
            var theContainer = new Container(x =>
            {
                x.IncludeRegistry<RavenDbRegistry>();
                x.ConnectToRavenDb<SecondDbSettings>();
                x.ConnectToRavenDb<ThirdDbSettings>();

                x.For<SecondDbSettings>().Use(new SecondDbSettings {RunInMemory = true});
                x.For<ThirdDbSettings>().Use(new ThirdDbSettings {RunInMemory = true});
            });

            var store2a = theContainer.GetInstance<IDocumentStore<SecondDbSettings>>();
            var store3a = theContainer.GetInstance<IDocumentStore<ThirdDbSettings>>();

            theContainer.GetInstance<IPersistenceReset>().ClearPersistedState();

            theContainer.GetInstance<IDocumentStore<SecondDbSettings>>()
                .ShouldNotBeTheSameAs(store2a);


            theContainer.GetInstance<IDocumentStore<ThirdDbSettings>>()
                .ShouldNotBeTheSameAs(store3a);

            var newSettings = theContainer.GetInstance<SecondDbSettings>();
            newSettings
                .RunInMemory.ShouldBeTrue();

            newSettings.Url.ShouldBeNull();
            newSettings.ConnectionString.ShouldBeNull();
        }
    }
}
