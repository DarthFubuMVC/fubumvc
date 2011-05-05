using System;
using System.Runtime.Serialization;
using FubuMVC.Core.Runtime;
using Microsoft.Practices.ServiceLocation;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.View.Activation
{
    [Serializable]
    public class PageActivationException : Exception
    {
        public PageActivationException(string message) : base(message)
        {
        }

        protected PageActivationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class SetPageModelActivationAction<T> : IPageActivationAction where T : class
    {
        public void Activate(IServiceLocator services, IFubuPage page)
        {
            var modelPage = (IFubuPage<T>)page;
            var request = services.GetInstance<IFubuRequest>();
            modelPage.Model = request.Get<T>() ?? request.Find<T>().FirstOrDefault();

//            if (modelPage.Model == null)
//            {
//                throw new PageActivationException("Could not find the requested view model of type {0} for page {1}".ToFormat(typeof(T).FullName, page.GetType().FullName));
//            }
        }
    }
}