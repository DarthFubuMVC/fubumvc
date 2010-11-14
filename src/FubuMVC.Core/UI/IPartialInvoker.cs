namespace FubuMVC.Core.UI
{
    public interface IPartialInvoker
    {
        void Invoke<T>() where T : class;
    }
}