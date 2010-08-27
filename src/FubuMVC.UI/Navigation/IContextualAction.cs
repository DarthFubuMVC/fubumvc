using FubuMVC.Core;

namespace FubuMVC.UI.Navigation
{
    public interface IContextualAction<in T>
    {
        string Key { get; }
        string Category { get; }
        MenuItemState UnauthorizedState { get; }
        string Text();
        MenuItemState IsAvailable(T target);
        Endpoint FindEndpoint(IEndpointService endpoints, T target);
    }
}