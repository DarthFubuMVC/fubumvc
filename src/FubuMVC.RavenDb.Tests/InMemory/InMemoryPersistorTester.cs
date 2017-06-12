using System.Linq;
using FubuMVC.RavenDb.InMemory;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests.InMemory
{
    public class InMemoryPersistorTester
    {
        [Fact]
        public void load_all()
        {
            var persistor = new InMemoryPersistor();

            persistor.Persist(new User());
            persistor.Persist(new User());
            persistor.Persist(new User());
            persistor.Persist(new OtherEntity());
            persistor.Persist(new OtherEntity());
            persistor.Persist(new ThirdEntity());

            persistor.LoadAll<User>().Count().ShouldBe(3);
            persistor.LoadAll<OtherEntity>().Count().ShouldBe(2);
            persistor.LoadAll<ThirdEntity>().Count().ShouldBe(1);
        }

        [Fact]
        public void persist()
        {
            var entity = new OtherEntity();
            var persistor = new InMemoryPersistor();

            persistor.Persist(entity);

            persistor.LoadAll<OtherEntity>().Single().ShouldBeTheSameAs(entity);
        }

        [Fact]
        public void delete_all()
        {
            var persistor = new InMemoryPersistor();

            persistor.Persist(new User());
            persistor.Persist(new User());
            persistor.Persist(new User());
            persistor.Persist(new OtherEntity());
            persistor.Persist(new OtherEntity());
            persistor.Persist(new ThirdEntity());

            persistor.DeleteAll<ThirdEntity>();

            persistor.LoadAll<User>().Count().ShouldBe(3);
            persistor.LoadAll<OtherEntity>().Count().ShouldBe(2);
            persistor.LoadAll<ThirdEntity>().Count().ShouldBe(0);
        }

        [Fact]
        public void remove()
        {
            var persistor = new InMemoryPersistor();

            persistor.Persist(new User());
            var user1 = new User();
            persistor.Persist(user1);
            persistor.Persist(new User());

            persistor.Remove(user1);

            persistor.LoadAll<User>().Count().ShouldBe(2);
            persistor.LoadAll<User>().ShouldNotContain(user1);
        }

        [Fact]
        public void find_by()
        {
            var persistor = new InMemoryPersistor();

            persistor.Persist(new User());
            persistor.Persist(new User
            {
                FirstName = "Jeremy"
            });
            persistor.Persist(new User());

            persistor.FindBy<User>(x => x.FirstName == "Jeremy").FirstName.ShouldBe("Jeremy");
        }
    }
}
