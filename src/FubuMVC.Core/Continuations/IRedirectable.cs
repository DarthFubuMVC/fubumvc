using System;
using System.Linq.Expressions;
using System.Net;

namespace FubuMVC.Core.Continuations
{
    /// <summary>
    /// Marker interface that can be used to redirect or
    /// transfer requests after an action by checking the 
    /// RedirectTo property
    /// </summary>
    public interface IRedirectable
    {
        FubuContinuation RedirectTo { get; set;}
    }

    /// <summary>
    /// Use to return an IRedirectable that stops the current
    /// request with an Http status code
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public static class Stop<TModel> where TModel : IRedirectable, new()
    {
        public static TModel With(HttpStatusCode code)
        {
            return new TModel{
                RedirectTo = FubuContinuation.EndWithStatusCode(code)
            };
        }
    }

    /// <summary>
    /// Return an IRedirectable that issues a redirect to the 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public static class Redirect<TModel> where TModel : IRedirectable, new()
    {
        /// <summary>
        /// Redirects the request to url from IUrlRegistry.UrlFor(destination)
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static TModel To(object destination)
        {
            return new TModel(){
                RedirectTo = FubuContinuation.RedirectTo(destination)
            };
        }

        /// <summary>
        /// Redirects the request to the url from IUrlRegistry.UrlFor<TActionHandler>(method)
        /// </summary>
        /// <typeparam name="TActionHandler"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static TModel To<TActionHandler>(Expression<Action<TActionHandler>> method)
        {
            return new TModel(){
                RedirectTo = FubuContinuation.RedirectTo(method)
            };
        }
    }

    /// <summary>
    /// Returns an IRedirectable that transfers to another chain
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public static class Transfer<TModel> where TModel : IRedirectable, new()
    {
        /// <summary>
        /// Transfers the request to url from IUrlRegistry.UrlFor(destination)
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static TModel To(object destination)
        {
            return new TModel()
            {
                RedirectTo = FubuContinuation.TransferTo(destination)
            };
        }

        /// <summary>
        /// Transfers the request to the url from IUrlRegistry.UrlFor<TActionHandler>(method)
        /// </summary>
        /// <typeparam name="TActionHandler"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static TModel To<TActionHandler>(Expression<Action<TActionHandler>> method)
        {
            return new TModel()
            {
                RedirectTo = FubuContinuation.TransferTo(method)
            };
        }
    }
}