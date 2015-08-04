using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.Sagas
{
    [Description("Adds saga handling to handler actions that implement IStatefulSaga<T>")]
    public class StatefulSagaConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var sagaHandlers = graph.Chains.SelectMany(x => x).OfType<HandlerCall>()
                                    .Where(IsSagaHandler)
                                    .ToArray();

            var settings = graph.Settings.Get<TransportSettings>();

            sagaHandlers.Each(call => {
                var types = ToSagaTypes(call);

                var sagaNode = new StatefulSagaNode(types)
                {
                    Repository = DetermineSagaRepositoryInstance(settings, types)
                };

                call.AddBefore(sagaNode);
            });
        }


        public static bool IsSagaHandler(HandlerCall call)
        {
            var handlerType = call.HandlerType;
            return IsSagaHandler(handlerType);
        }

        public static bool IsSagaHandler(Type handlerType)
        {
            return handlerType.Closes(typeof (IStatefulSaga<>));
        }

        public static bool IsSagaChain(BehaviorChain chain)
        {
            if (chain is HandlerChain)
            {
                return chain.OfType<HandlerCall>().Any(IsSagaHandler);
            }

            return false;
        }

        public static Instance DetermineSagaRepositoryInstance(TransportSettings settings, SagaTypes sagaTypes)
        {
            var def = settings.SagaStorageProviders.FirstValue(x => x.RepositoryFor(sagaTypes))
                      ?? new InMemorySagaStorage().RepositoryFor(sagaTypes);

            if (def == null)
            {
                throw new SagaRepositoryUnresolvableException(sagaTypes);
            }

            return def;
        }

        public static SagaTypes ToSagaTypes(HandlerCall call)
        {
            return new SagaTypes
            {
                HandlerType = call.HandlerType,
                MessageType = call.InputType(),
                StateType = call.HandlerType.FindInterfaceThatCloses(typeof(IStatefulSaga<>)).GetGenericArguments().Single()
            };
        }


    }
}