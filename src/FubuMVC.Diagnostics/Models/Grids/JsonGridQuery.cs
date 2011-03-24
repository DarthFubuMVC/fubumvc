using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models.Grids
{
	public class JsonGridQuery
    {
        public const string ASCENDING = "asc";
        public const string DESCENDING = "desc";

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

		public static JsonGridQuery ForFilter(JsonGridFilter filter)
		{
			return new JsonGridQuery
			       	{
						Filters = new List<JsonGridFilter> { filter },
						page = 0,
						rows = -1,
						sidx = filter.ColumnName,
						sord = JsonGridQuery.ASCENDING
			       	};
		}
    }
}