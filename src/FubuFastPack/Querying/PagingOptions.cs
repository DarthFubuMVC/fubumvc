using System.Collections.Generic;
using FubuCore;
using System.Linq;

namespace FubuFastPack.Querying
{
    // Rename to QueryOptions and make a reference to criteria
    public class PagingOptions
    {
        private readonly IList<Criteria> _criterion = new List<Criteria>();

        public PagingOptions()
        {
        }

        public PagingOptions(int page, int resultsPerPage, string sortColumn, bool sortAscending)
        {
            Page = page > 0 ? page : 1;
            ResultsPerPage = resultsPerPage;
            SortColumn = sortColumn;
            SortAscending = sortAscending;
        }

        public int Page { get; set; }
        public int ResultsPerPage { get; set; }
        public string SortColumn { get; set; }
        public bool SortAscending { get; set; }

        public Criteria[] Criterion
        {
            get
            {
                return _criterion.ToArray();
            }
            set
            {
                _criterion.Clear();
                _criterion.AddRange(value);
            }
        }

        public int ResultsToSkip()
        {
            return (Page - 1) * ResultsPerPage;
        }

        public override string ToString()
        {
            return "{{Paging: Page {0}, Rows/Page: {1}, Sort: {2}, Asc?: {3}}}".ToFormat(Page, ResultsPerPage,
                                                                                         SortColumn, SortAscending);
        }


        public bool Equals(PagingOptions obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.Page == Page && obj.ResultsPerPage == ResultsPerPage && Equals(obj.SortColumn, SortColumn) &&
                   obj.SortAscending.Equals(SortAscending);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(PagingOptions)) return false;
            return Equals((PagingOptions)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Page;
                result = (result * 397) ^ ResultsPerPage;
                result = (result * 397) ^ (SortColumn != null ? SortColumn.GetHashCode() : 0);
                result = (result * 397) ^ SortAscending.GetHashCode();
                return result;
            }
        }
    }
}