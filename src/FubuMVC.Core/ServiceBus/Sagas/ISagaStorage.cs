using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.Sagas
{
    public interface ISagaStorage
    {
        /// <summary>
        /// Can be null!
        /// </summary>
        /// <param name="sagaTypes"></param>
        /// <returns></returns>
        Instance RepositoryFor(SagaTypes sagaTypes);
    }
}