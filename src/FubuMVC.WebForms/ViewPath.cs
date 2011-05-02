namespace FubuMVC.WebForms
{
    public class ViewPath
    {
        public string ViewName { get; set; }

        public bool Equals(ViewPath other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ViewName, ViewName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ViewPath)) return false;
            return Equals((ViewPath) obj);
        }

        public override int GetHashCode()
        {
            return (ViewName != null ? ViewName.GetHashCode() : 0);
        }
    }
}