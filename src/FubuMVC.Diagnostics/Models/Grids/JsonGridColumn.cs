namespace FubuMVC.Diagnostics.Models.Grids
{
    public class JsonGridColumn
    {
        public JsonGridColumn()
        {
            Value = string.Empty;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsIdentifier { get; set; }
    }
}