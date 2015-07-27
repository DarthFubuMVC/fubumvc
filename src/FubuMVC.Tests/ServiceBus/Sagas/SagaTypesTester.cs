using System;
using FubuMVC.Core.ServiceBus.Sagas;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Sagas
{
    [TestFixture]
    public class SagaTypesTester
    {
        [Test]
        public void saga_types_gives_an_empty_guid_func_if_correlation_id_does_not_exist()
        {
            var types = new SagaTypes
            {
                MessageType = GetType()
            };

            var func = types.ToCorrelationIdFunc().ShouldBeOfType<Func<SagaTypesTester, Guid>>();
            func(this).ShouldBe(Guid.Empty);
        }

        [Test]
        public void saga_types_being_able_to_gimme_a_correlation_id_getter_from_the_message_type()
        {
            var types = new SagaTypes
            {
                MessageType = typeof(SagaMessageOne)
            };

            var func = types.ToCorrelationIdFunc().ShouldBeOfType<Func<SagaMessageOne, Guid>>();

            var message = new SagaMessageOne
            {
                CorrelationId = Guid.NewGuid()
            };

            func(message).ShouldBe(message.CorrelationId);
        }

        [Test]
        public void saga_types_being_able_to_gimme_an_id_getter_for_the_state_object()
        {
            var types = new SagaTypes
            {
                MessageType = typeof(SagaMessageOne),
                StateType = typeof(MySagaState)
            };

            var func = types.ToSagaIdFunc().ShouldBeOfType<Func<MySagaState, Guid>>();

            var state = new MySagaState
            {
                Id = Guid.NewGuid()
            };

            func(state).ShouldBe(state.Id);

        }

        [Test]
        public void saga_types_matches_idiom()
        {
            new SagaTypes
            {
                MessageType = typeof(SagaMessageOne),
                StateType = typeof(MySagaState)
            }.MatchesStateIdAndMessageCorrelationIdIdiom().ShouldBeTrue();
        }

        [Test]
        public void saga_types_does_not_match_idiom_because_of_state_type_not_having_id()
        {
            new SagaTypes
            {
                MessageType = typeof(SagaMessageOne),
                StateType = GetType()
            }.MatchesStateIdAndMessageCorrelationIdIdiom().ShouldBeFalse();
        }

        [Test]
        public void saga_types_does_not_match_idiom_because_of_message_type_not_having_correlation_id()
        {
            new SagaTypes
            {
                MessageType = GetType(),
                StateType = typeof(MySagaState)
            }.MatchesStateIdAndMessageCorrelationIdIdiom().ShouldBeFalse();
        }

    }
}