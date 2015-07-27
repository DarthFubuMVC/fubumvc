using System;
using FubuMVC.Core.ServiceBus.Sagas;

namespace FubuMVC.Tests.ServiceBus.Docs.Sagas
{
    // SAMPLE: OverviewSagaSample
    public class SimpleSagaHandler : IStatefulSaga<SimpleSagaState>
    {
        public SimpleSagaState State { get; set; }

        public bool IsCompleted()
        {
            return State.Switch == SwitchState.Exit;
        }

        public void Handle(StartCommand command)
        {
            State = new SimpleSagaState {Switch = SwitchState.Active};
        }

        public void Handle(PauseCommand command)
        {
            State.Switch = SwitchState.Paused;
        }

        public void Handle(ResumeCommand command)
        {
            if (State.Switch == SwitchState.Paused)
            {
                State.Switch = SwitchState.Paused;
            }
        }

        public void Handle(EndCommand command)
        {
            State.Switch = SwitchState.Inactive;
        }

        public void Handle(ExitCommand command)
        {
            if (State.Switch == SwitchState.Inactive)
            {
                State.Switch = SwitchState.Exit;
            }
        }
    }

    public class SimpleSagaState
    {
        public SimpleSagaState()
        {
            Switch = SwitchState.Inactive;
        }

        public Guid Id { get; set; }
        public SwitchState Switch { get; set; }
    }

    public enum SwitchState
    {
        Inactive,
        Active,
        Paused,
        Exit
    }

    public class StartCommand
    {
        public Guid CorrelationId { get; set; }
    }

    public class PauseCommand
    {
        public Guid CorrelationId { get; set; }
    }

    public class ResumeCommand
    {
        public Guid CorrelationId { get; set; }
    }

    public class EndCommand
    {
        public Guid CorrelationId { get; set; }
    }

    public class ExitCommand
    {
        public Guid CorrelationId { get; set; }
    }
    // ENDSAMPLE
}