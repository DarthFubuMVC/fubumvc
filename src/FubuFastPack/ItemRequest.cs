using System;
using FubuMVC.Core;

namespace FubuFastPack
{
    public class ItemRequest : IItemRequest
    {
        [RouteInput]
        public Guid Id { get; set; }

        public bool Equals(ItemRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id.Equals(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ItemRequest)) return false;
            return Equals((ItemRequest)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public interface IItemRequest
    {
        Guid Id { get; set; }
    }
}