using System.Collections.Generic;
using FubuCore.Logging;
using FubuMVC.RavenDb.Reset;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.RavenDb.Tests.Reset
{
    [TestFixture]
    public class CompleteResetTester
    {
        private CompleteReset theCompleteReset;

        [SetUp]
        public void SetUp()
        {
            Tracer.Messages.Clear();

            theCompleteReset = new CompleteReset(MockRepository.GenerateMock<ILogger>(), new FakeInitialState(), 
                                                 new FakePersistenceReset(),
                                                 new IServiceReset[]
                                                 {
                                                     new FakeServiceReset("A"), new FakeServiceReset("B"),
                                                     new FakeServiceReset("C")
                                                 });
        }

        [Test]
        public void clearing_state_order()
        {
            theCompleteReset.ResetState();
            Tracer.Messages.ShouldHaveTheSameElementsAs("Service:Stop:A", "Service:Stop:B", "Service:Stop:C", "PersistenceReset:ClearPersistedState", "IInitialState:Load", "Service:Start:A", "Service:Start:B", "Service:Start:C");
        }

        [Test]
        public void committing_the_changes()
        {
            theCompleteReset.CommitChanges();

            Tracer.Messages.ShouldHaveTheSameElementsAs("PersistenceReset:CommitAllChanges");
        }
    }

    public class FakePersistenceReset : IPersistenceReset
    {
        public void ClearPersistedState()
        {
            Tracer.Messages.Add("PersistenceReset:ClearPersistedState");
        }

        public void CommitAllChanges()
        {
            Tracer.Messages.Add("PersistenceReset:CommitAllChanges");
        }
    }

    public class FakeInitialState : IInitialState
    {
        public void Load()
        {
            Tracer.Messages.Add("IInitialState:Load");
        }
    }

    public class FakeServiceReset : IServiceReset
    {
        private readonly string _name;

        public FakeServiceReset(string name)
        {
            _name = name;
        }

        public void Stop()
        {
            Tracer.Messages.Add("Service:Stop:" + _name);
        }

        public void Start()
        {
            Tracer.Messages.Add("Service:Start:" + _name);
        }
    }

    public static class Tracer
    {
        public static IList<string> Messages = new List<string>();
    }
}