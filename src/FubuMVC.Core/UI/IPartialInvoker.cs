namespace FubuMVC.Core.UI
{
    public interface IPartialInvoker
    {
        string Invoke<T>() where T : class;
        string InvokeObject(object model);
    }
}