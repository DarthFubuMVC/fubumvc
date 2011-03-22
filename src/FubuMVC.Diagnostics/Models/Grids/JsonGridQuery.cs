using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models.Grids
{
    public class JsonGridQuery
    {
        public const string ASCENDING = "asc";
        public const string DESCENDING = "desc";
    }

    // Open generic so we can close and keep unique input models
    public class JsonGridQuery<T> : JsonGridQuery
    {
        public JsonGridQuery()
        {
            Filters = new List<JsonGridFilter>();
            sord = ASCENDING;
        }

        // matches jqGrid post params
        public int page { get; set; }
        public int rows { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }

        public IEnumerable<JsonGridFilter> Filters { get; set; }
    }
}