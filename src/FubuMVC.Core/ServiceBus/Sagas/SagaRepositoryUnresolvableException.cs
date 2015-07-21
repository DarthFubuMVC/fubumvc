using System;
using System.Runtime.Serialization;
using FubuCore;

namespace FubuMVC.Core.ServiceBus.Sagas
{
    [Serializable]
    public class SagaRepositoryUnresolvableException : Exception
    {
        public SagaRepositoryUnresolvableException(SagaTypes sagaTypes) : base("Unable to determine a saga repository for {0}.  Does the saga type have a property Id:Guid and the message type a property of CorrelationId:Guid?".ToFormat(sagaTypes))
        {
        }

        protected SagaRepositoryUnresolvableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}