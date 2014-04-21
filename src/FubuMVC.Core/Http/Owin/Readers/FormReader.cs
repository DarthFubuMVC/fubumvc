using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http.Owin.Readers
{
    public class FormReader : IOwinRequestReader
    {
        public void Read(IDictionary<string, object> environment)
        {
            var form = new NameValueCollection();

            environment.Add(OwinConstants.RequestFormKey, form);
            var mediaType = environment.Get<string>(OwinConstants.MediaTypeKey);
            if (mediaType != MimeType.HttpFormMimetype && mediaType != MimeType.MultipartMimetype)  return; 

            string formValue;
            var body = environment.Get<Stream>(OwinConstants.RequestBodyKey);
            using (var reader = new StreamReader(body))
            {
                formValue = reader.ReadToEnd();
            }

            form.Add(HttpUtility.ParseQueryString(formValue));
        }
    }
}