namespace FubuTransportation.Sagas
{
    public interface IStatefulSaga<TState>
    {
        TState State { get; set; }
        bool IsCompleted();
    }
}