using System;
using FubuCore.Dates;

namespace FubuPersistence
{
    public abstract class Entity : IEntity
    {
        public Guid Id { get; set; }

        public bool Equals(Entity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id.Equals(Id) && other.GetType() == GetType();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Entity)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}