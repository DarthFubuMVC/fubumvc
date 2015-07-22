using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.Sagas
{
    public class StatefulSagaNode : BehaviorNode, DescribesItself
    {
        private readonly SagaTypes _types;

        public StatefulSagaNode(SagaTypes types)
        {
            _types = types;
        }

        public Instance Repository { get; set; }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Wrapper; }
        }

        public Type StateType
        {
            get { return _types.StateType; }
        }

        public Type MessageType
        {
            get { return _types.MessageType; }
        }


        protected override IConfiguredInstance buildInstance()
        {
            if (Repository == null)
            {
                throw new InvalidOperationException(
                    "You must specify a SagaRepository");
            }

            if (Repository == null)
            {
                throw new InvalidOperationException(
                    "something descriptive here saying you don't know how to do the repo for the saga");
            }

            

            var instance = new ConfiguredInstance(typeof (SagaBehavior<,,>), _types.StateType, _types.MessageType, _types.HandlerType);
            var repositoryType = typeof (ISagaRepository<,>).MakeGenericType(_types.StateType, _types.MessageType);

            instance.Dependencies.Add(repositoryType, Repository);

            return instance;

        }

        public void Describe(Description description)
        {
            description.Title = "Stateful Saga Node";
            description.ShortDescription = "Applies saga state loading and persistence within this chain";
            description.Properties["State Type"] = StateType.FullName;
            description.Properties["Message Type"] = MessageType.FullName;
            description.AddChild("Repository", Repository);
        }
    }
}