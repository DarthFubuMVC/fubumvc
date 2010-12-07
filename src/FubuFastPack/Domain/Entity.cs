using System;
using FubuCore.Reflection;

namespace FubuFastPack.Domain
{
    // Move Entity
    // Move DomainEntity as is
    [Serializable]
    public class Entity : IEquatable<Entity>
    {
        static Entity()
        {
            IdPropertyName = ReflectionHelper.GetProperty<Entity>(x => x.Id).Name;
        }
        public static string IdPropertyName { get; private set; }

        public const int UnboundedStringLength = 200000;
        public const int DefaultVarcharLength = 255;

        public virtual Guid Id { get; set; }

        public virtual bool Equals(Entity obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj as Entity == null) return false;
            return Equals((Entity)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !Equals(left, right);
        }
    }


    // TODO -- need to pull over unit tests here
}