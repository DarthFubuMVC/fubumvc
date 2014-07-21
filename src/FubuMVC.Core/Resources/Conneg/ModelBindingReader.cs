using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime;
using FubuCore;

namespace FubuMVC.Core.Resources.Conneg
{
    public class ModelBindingReader<T> : IReader<T>, DescribesItself where T : class
    {
        public T Read(string mimeType, IFubuRequestContext context)
        {
            context.Models.Clear(typeof(T));
            return context.Models.Get<T>();
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return MimeType.HttpFormMimetype;
                yield return MimeType.MultipartMimetype;
            }
        }

        public void Describe(Description description)
        {
            description.Title = "Read {0} by model binding against the request data".ToFormat(typeof (T).Name);
            description.ShortDescription = "Read {0} by model binding against the request data".ToFormat(typeof(T).Name);
        }

        public Type ModelType
        {
            get
            {
                return typeof (T);
            }
        }
    }
}