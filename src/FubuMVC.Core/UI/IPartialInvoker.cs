namespace FubuMVC.Core.UI
{
    public interface IPartialInvoker
    {
        string Invoke<T>(string categoryOrHttpMethod = null) where T : class;
        string InvokeObject(object model, bool withModelBinding = false, string categoryOrHttpMethod = null);

        string InvokeAsHtml(object model);
    }
}