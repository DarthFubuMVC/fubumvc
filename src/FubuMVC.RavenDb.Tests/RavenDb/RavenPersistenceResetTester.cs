using System.Linq;
using System.Threading;
using FubuCore;
using FubuMVC.RavenDb.RavenDb;
using FubuMVC.RavenDb.RavenDb.Multiple;
using FubuMVC.RavenDb.Reset;
using FubuMVC.RavenDb.Tests.RavenDb.Integration;
using NUnit.Framework;
using Shouldly;
using StructureMap;
using Process = System.Diagnostics.Process;

namespace FubuMVC.RavenDb.Tests.RavenDb
{
    [TestFixture]
    public class RavenPersistenceResetTester
    {
        private Container container;
        private RavenUnitOfWork theUnitOfWork;
        private IPersistenceReset theReset;

        [SetUp]
        public void SetUp()
        {
            container = new Container(new RavenDbRegistry());
            container.Inject(new RavenDbSettings { RunInMemory = true });

            theReset = container.GetInstance<IPersistenceReset>();
            theUnitOfWork = new RavenUnitOfWork(container);
        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        [Test, Explicit("Manual only testing")]
        public void can_access_the_new_store_by_url()
        {
            theReset.ClearPersistedState();
            Process.Start("http://localhost:8080");
            Thread.Sleep(60000);
        }

        [Test]
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

        [Test]
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
    }

    [TestFixture, Explicit("some cleanup problems here.")]
    public class when_clearing_persisted_state_with_multiple_settings
    {
        [Test]
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