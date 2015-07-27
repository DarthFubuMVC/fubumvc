using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.RavenDb.Tests
{
    [TestFixture]
    public class EntityRepositoryTester
    {

        [Test]
        public void assign_a_guid_if_one_does_not_exist()
        {
            var repository = EntityRepository.InMemory();
            var @case = new FakeEntity();

            @case.Id.ShouldBe(Guid.Empty);

            repository.Update(@case);

            @case.Id.ShouldBeOfType<Guid>().ShouldNotBe(Guid.Empty);
        }

        [Test]
        public void update_an_existing_case_should_replace_it()
        {
            var repository = EntityRepository.InMemory();
            var case1 = new FakeEntity();
            var case2 = new FakeEntity();

            repository.Update(case1);
            repository.All<FakeEntity>().Single().ShouldBeTheSameAs(case1);
            repository.Find<FakeEntity>(case1.Id).ShouldBeTheSameAs(case1);

            case2.Id = case1.Id;
            repository.Update(case2);

            repository.All<FakeEntity>().Single().ShouldBeTheSameAs(case2);
            repository.Find<FakeEntity>(case1.Id).ShouldBeTheSameAs(case2);
        }

        [Test]
        public void fetch_is_covariant_contravariant_you_know_what_I_mean()
        {
            var repository = EntityRepository.InMemory();
            var case1 = new FakeEntity();
            repository.Update(case1);

            repository.Find<FakeEntity>(case1.Id).ShouldBeTheSameAs(case1);
        }
    }
}