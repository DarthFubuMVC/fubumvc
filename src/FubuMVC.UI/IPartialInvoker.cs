namespace FubuMVC.UI
{
    public interface IPartialInvoker
    {
        void Invoke<T>() where T : class;
    }
}