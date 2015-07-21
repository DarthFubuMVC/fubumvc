namespace FubuMVC.Core.ServiceBus.Sagas
{
    public interface IStatefulSaga<TState>
    {
        TState State { get; set; }
        bool IsCompleted();
    }
}