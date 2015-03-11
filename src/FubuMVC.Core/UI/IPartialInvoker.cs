using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.UI
{
    public interface IPartialInvoker
    {
        string Invoke<T>(string categoryOrHttpMethod = null) where T : class;
        string InvokeObject(object model, bool withModelBinding = false, string categoryOrHttpMethod = null);

        string InvokeAsHtml(object model);

        /// <summary>
        /// Invokes a partial request with the conneg output disabled for
        /// the partial request
        /// </summary>
        /// <param name="chain"></param>
        void InvokeFast(BehaviorChain chain);
    }
}