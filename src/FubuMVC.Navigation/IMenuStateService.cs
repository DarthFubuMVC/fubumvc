namespace FubuMVC.Navigation
{
    public interface IMenuStateService
    {
        MenuItemState DetermineStateFor(MenuNode node);
    }
}