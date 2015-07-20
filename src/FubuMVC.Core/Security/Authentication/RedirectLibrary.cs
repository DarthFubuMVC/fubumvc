using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core.Security.Authentication
{
    [MarkedForTermination]
    public class RedirectLibrary
    {
        private readonly IList<Type> _redirectTypes = new List<Type>();

        public void Add<T>() where T : IAuthenticationRedirect
        {
            _redirectTypes.Add(typeof(T));
        }

        public IEnumerable<Type> GetRedirectTypes()
        {
            foreach (var redirectType in _redirectTypes)
            {
                yield return redirectType;
            }

            yield return typeof (AjaxAuthenticationRedirect);
            yield return typeof (DefaultAuthenticationRedirect);
        } 
    }
}