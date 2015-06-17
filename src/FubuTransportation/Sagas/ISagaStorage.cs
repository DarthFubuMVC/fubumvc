using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuTransportation.Sagas
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