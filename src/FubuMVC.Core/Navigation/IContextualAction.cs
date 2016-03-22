using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Navigation
{
    public interface IContextualAction<in T>
    {
        string Key { get; }
        string Category { get; }
        MenuItemState UnauthorizedState { get; }
        string Text();
		string Description();
        MenuItemState IsAvailable(T target);
        Endpoint FindEndpoint(IEndpointService endpoints, T target);
    }
}