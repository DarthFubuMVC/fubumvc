namespace FubuMVC.Diagnostics.Notifications
{
	public class FilterLink
	{
		private readonly string _columnName;
		private readonly string _value;

		public FilterLink(string columnName, string value)
		{
			_columnName = columnName;
			_value = value;
		}

		public string Value
		{
			get { return _value; }
		}

		public string ColumnName
		{
			get { return _columnName; }
		}
	}
}