using System;

namespace FubuMVC.Core.Http
{
    public interface IRequestHeaders
    {
        /// <summary>
        ///   Retrieve the value of a single Header property.  
        ///   The callback action will only be called if the Header
        ///   value exists
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "header"></param>
        /// <param name = "callback"></param>
        void Value<T>(string header, Action<T> callback);

        /// <summary>
        ///   Bind an object of type T to the data in the Headers
        ///   collection
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <returns></returns>
        T BindToHeaders<T>();


        bool HasHeader(string header);
    }
}