using System.Reflection;
using System.Text;
using System.Web;
using System.Collections.Generic;

namespace AspNetApplication.WebForms
{
    public class WebFormInput
    {
        public string Name { get; set; }
    }

    public class AspNetModelBindingInput
    {
        //public string[] AcceptTypes { get; set; } // NOTE: There's a bug binding arrays like this since it triggers the array/collection binding when we don't want it to
        public Encoding ContentEncoding { get; set; }
        public HttpBrowserCapabilitiesWrapper Browser { get; set; }
        public int ContentLength { get; set; }
        public HttpCookieCollection Cookies { get; set; }
        public string UserAgent { get; set; }
        public string User_Agent { get; set; }

        public string AsString()
        {
            var builder = new StringBuilder();
            //builder.AppendFormat("<br/>AcceptTypes: ['{0}']", AcceptTypes.Join("','"));
            builder.AppendFormat("<br/>Encoding: {0}", ContentEncoding.WebName);
            builder.AppendFormat("<br/>Browser: {0}", (Browser == null) ? "null" : Browser.Type);
            builder.AppendFormat("<br/>ContentLength: {0}", ContentLength);
            builder.AppendFormat("<br/>Cookies: {0}", Cookies.Count);
            builder.AppendFormat("<br/>HttpRequest.UserAgent: {0}", UserAgent);
            builder.AppendFormat("<br/>HttpRequest[\"User-Agent\"]: {0}", User_Agent);

            return builder.ToString();
        }

    }

    public class WebFormOutput
    {
        public string Text { get; set; }
    }

    public class ViewController
    {
        public WebFormOutput get_webforms_simple_Name(WebFormInput input)
        {
            return new WebFormOutput{
                Text = "My name is " + input.Name
            };
        }

        public WebFormOutput post_webforms_simple_Name(WebFormInput input)
        {
            return new WebFormOutput
            {
                Text = "My name is " + input.Name
            };
        }

        public WebFormOutput get_webforms_simple_binding(AspNetModelBindingInput input)
        {
            return new WebFormOutput
                       {
                           Text = input.AsString()
                       };

        }
    }
}