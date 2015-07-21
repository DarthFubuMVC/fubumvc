using System;
using System.Security.Cryptography;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
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

        public ObjectDef Repository { get; set; }

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

        protected override ObjectDef buildObjectDef()
        {
            if (Repository == null)
            {
                throw new InvalidOperationException(
                    "something descriptive here saying you don't know how to do the repo for the saga");
            }

            var def = new ObjectDef(typeof (SagaBehavior<,,>), _types.StateType, _types.MessageType, _types.HandlerType);
            var repositoryType = typeof (ISagaRepository<,>).MakeGenericType(_types.StateType, _types.MessageType);
            def.Dependency(repositoryType, Repository);

            return def;
        }

        protected override IConfiguredInstance buildInstance()
        {
            if (Repository == null)
            {
                throw new InvalidOperationException(
                    "You must specify a SagaRepository");
            }

            throw new NotImplementedException("Do later");

            /*
            if (Repository == null)
            {
                throw new InvalidOperationException(
                    "something descriptive here saying you don't know how to do the repo for the saga");
            }

            var def = new ObjectDef(typeof (SagaBehavior<,,>), _types.StateType, _types.MessageType, _types.HandlerType);
            var repositoryType = typeof (ISagaRepository<,>).MakeGenericType(_types.StateType, _types.MessageType);
            def.Dependency(repositoryType, Repository);

            return def;
             */
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