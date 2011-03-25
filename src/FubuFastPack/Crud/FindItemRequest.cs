namespace FubuFastPack.Crud
{
    public class FindItemRequest<TEntity> : ItemRequest
    {
        public bool Equals(FindItemRequest<TEntity> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id.Equals(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(FindItemRequest<TEntity>)) return false;
            return Equals((FindItemRequest<TEntity>)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}