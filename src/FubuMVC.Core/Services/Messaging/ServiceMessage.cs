namespace FubuMVC.Core.Services.Messaging
{
    public class ServiceMessage
    {
        public string Category { get; set; }
        public string Message { get; set; }

        protected bool Equals(ServiceMessage other)
        {
            return string.Equals(Category, other.Category) && string.Equals(Message, other.Message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ServiceMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Category != null ? Category.GetHashCode() : 0)*397) ^ (Message != null ? Message.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Category: {0}, Message: {1}", Category, Message);
        }
    }
}