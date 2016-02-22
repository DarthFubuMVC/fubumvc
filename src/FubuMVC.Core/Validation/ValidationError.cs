namespace FubuMVC.Core.Validation
{
    public class ValidationError
    {
        public ValidationError()
        {
        }

        public ValidationError(string field, string message)
            : this(field, field, message)
        {
            
        }

        public ValidationError(string field, string label, string message)
        {
            this.field = field;
            this.label = label;
            this.message = message;
        }

        public string field { get; set; }
        public string message { get; set; }
        public string label { get; set; }

        public bool Equals(ValidationError other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.field, field) && Equals(other.message, message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ValidationError)) return false;
            return Equals((ValidationError) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((field != null ? field.GetHashCode() : 0)*397) ^ (message != null ? message.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("field: {0}, message: {1}", field, message);
        }
    }
}