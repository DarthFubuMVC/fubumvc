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
            T model;
            //We need to see if there is an exact match first.
            if(request.Has<T>())
            {
                //We've got an exact match, call request.Get<T> knowing we'll get the exact match.
                model = request.Get<T>();
            }
            else
            {
                //We didn't find nan exact match, so we need to look for things that can be cast to the type requested.
                var found = request.Find<T>().FirstOrDefault();
                //If we don't find anything that satisfies the type request, call Get<T>() which will quietly create what we're looking for underneath the hood.
                model = found ?? request.Get<T>();
            }

            modelPage.Model = model;

//            if (modelPage.Model == null)
//            {
//                throw new PageActivationException("Could not find the requested view model of type {0} for page {1}".ToFormat(typeof(T).FullName, page.GetType().FullName));
//            }
        }
    }
}