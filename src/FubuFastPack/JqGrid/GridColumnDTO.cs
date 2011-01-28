namespace FubuFastPack.JqGrid
{
    // Keep this as a stupid data bag
// ReSharper disable InconsistentNaming
    public class GridColumnDTO
// ReSharper restore InconsistentNaming
    {
        public const string DataColumnFormatterType = "data";

        public GridColumnDTO(string name, string header)
        {
            Name = name;
            Header = header;
            Sortable = true;
            DisplayType = DataColumnFormatterType;
        }

        public string DisplayType { get; set; }
        public string Name { get; private set; }
        public string Header { get; private set; }
        public bool Sortable { get; set; }
        public string EditType { get; set; }

        public bool Equals(GridColumnDTO obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Name, Name) && Equals(obj.Header, Header) && obj.Sortable.Equals(Sortable) && Equals(obj.EditType, EditType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(GridColumnDTO)) return false;
            return Equals((GridColumnDTO)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result * 397) ^ (Header != null ? Header.GetHashCode() : 0);
                result = (result * 397) ^ Sortable.GetHashCode();
                result = (result * 397) ^ (EditType != null ? EditType.GetHashCode() : 0);
                return result;
            }
        }
    }
}