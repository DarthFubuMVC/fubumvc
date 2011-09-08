namespace FubuMVC.Diagnostics.Core.Grids.Columns
{
	public interface IGridColumn<T>
	{
		int Rank();
		string Name();
		string ValueFor(T target);
		bool IsIdentifier();
		bool IsHidden(T target);
		bool HideFilter(T target);
	}
}