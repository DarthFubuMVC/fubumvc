using System;
using System.Globalization;
using System.Text;
using FubuCore.Util;
using FubuCore;

namespace FubuMVC.Core.Http
{
    /*
     * 
     * Notes:
     * Need to sort the cookies
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */
    
    public class Cookie
    {
        private readonly Cache<string, string> _values = new Cache<string,string>();
        
        public string Domain { get; set; }
        public DateTime? Expires { get; set;}
        public bool HttpOnly { get; set; }
        public string Path { get; set; }
        public bool Secure { get; set; }

        /// <summary>
        /// This gives you read/write access to all the values by name
        /// </summary>
        public Indexer<string, string> Values
        {
            get
            {
                return new Indexer<string, string>(key => _values[key], (key, value) => _values[key] = value);
            }
        }

        //public string GetHeaderValue()
        //{
        //    var builder = new StringBuilder();
            
        //    _values.Each((key, value) => builder.AddCookieValue(key, value));


        //    Domain.IfNotNull(x => builder.AddCookieValue("domain", Domain));
        //    Expires.IfNotNull(x => builder.AddCookieValue("expires", x.FormatHttpCookieDateTime()));

        //    Path.IfNotNull(x => builder.AddCookieValue("path", x));

        //    if (Secure) builder.AddCookiePart("secure");

        //    if (HttpOnly) builder.AddCookiePart("HttpOnly");

        //    return builder.ToString().TrimEnd().TrimEnd(';');

        //}



        public void EachValue(Action<string, string> callback)
        {
            _values.Each(callback);
        }

        
 

    }

    public static class CookieStringBuilderExtensions
    {
        public static void AddCookiePart(this StringBuilder builder, string part)
        {
            builder.Append(part);
            builder.Append("; ");
        }

        public static void AddCookieValue(this StringBuilder builder, string name, string value)
        {
            builder.Append(name);
            builder.Append("=");
            builder.Append(value);
            builder.Append("; ");
        }

        public static string FormatHttpCookieDateTime(this DateTime dt)
        {
            if ((dt < DateTime.MaxValue.AddDays(-1.0)) && (dt > DateTime.MinValue.AddDays(1.0)))
            {
                dt = dt.ToUniversalTime();
            }
            return dt.ToString("ddd, dd-MMM-yyyy HH':'mm':'ss 'GMT'", DateTimeFormatInfo.InvariantInfo);
        }

    }
}