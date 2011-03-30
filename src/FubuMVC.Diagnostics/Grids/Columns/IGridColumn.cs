namespace FubuMVC.Diagnostics.Grids.Columns
{
	public interface IGridColumn<T>
	{
		string Name();
		string ValueFor(T target);
		bool IsIdentifier();
		bool IsHidden(T target);
		bool HideFilter(T target);
	}
}