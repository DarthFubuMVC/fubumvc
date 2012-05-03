namespace FubuMVC.Core.UI.Navigation
{
    public interface IMenuStateService
    {
        MenuItemState DetermineStateFor(MenuNode node);
    }
}