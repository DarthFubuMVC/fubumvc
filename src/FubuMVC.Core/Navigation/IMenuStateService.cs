namespace FubuMVC.Core.Navigation
{
    public interface IMenuStateService
    {
        MenuItemState DetermineStateFor(MenuNode node);
    }
}