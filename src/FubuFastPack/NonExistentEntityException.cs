using System;
using System.Runtime.Serialization;
using FubuCore;

namespace FubuFastPack
{
    [Serializable]
    public class NonExistentEntityException : Exception
    {
        private const string ENTITY_TYPE_KEY = "EntityType";
        private const string ENTITY_ID_KEY = "EntityId";

        public NonExistentEntityException()
        {
        }

        public NonExistentEntityException(Type entityType, Guid entityId)
        {
            EntityType = entityType.FullName;
            EntityId = entityId;
        }

        protected NonExistentEntityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            EntityType = info.GetString(ENTITY_TYPE_KEY);
            EntityId = new Guid(info.GetString(ENTITY_ID_KEY));
        }

        public string EntityType { get; private set; }
        public Guid EntityId { get; private set; }

        public override string Message
        {
            get { return "Could not find entity of type {0} with ID: {1}".ToFormat(EntityType, EntityId); }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(ENTITY_TYPE_KEY, EntityType);
            info.AddValue(ENTITY_ID_KEY, EntityId);
        }
    }
}