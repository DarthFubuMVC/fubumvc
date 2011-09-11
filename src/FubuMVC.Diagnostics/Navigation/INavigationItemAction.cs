namespace FubuMVC.Diagnostics.Navigation
{
    public interface INavigationItemAction
    {
        int Rank { get; }
        string Text();
        string Url();
    }
}