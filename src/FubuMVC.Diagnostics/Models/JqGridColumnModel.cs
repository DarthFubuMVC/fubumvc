using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models
{
    public class JqGridColumnModel
    {
        private readonly IList<JqGridColumn> _columns = new List<JqGridColumn>();

        public void AddColumn(JqGridColumn column)
        {
            _columns.Add(column);
        }

        public IEnumerable<JqGridColumn> Columns { get { return _columns; } }
    }
}