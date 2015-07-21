using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.ServiceBus.Sagas
{
    public interface ISagaStorage
    {
        /// <summary>
        /// Can be null!
        /// </summary>
        /// <param name="sagaTypes"></param>
        /// <returns></returns>
        ObjectDef RepositoryFor(SagaTypes sagaTypes);
    }
}