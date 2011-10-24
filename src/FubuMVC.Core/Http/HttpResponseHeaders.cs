using System.Net;
using FubuCore.Util;

namespace FubuMVC.Core.Http
{
    public class HttpResponseHeaders : HttpGeneralHeaders
    {
        public static readonly string AcceptRanges = "AcceptRanges";
        public static readonly string Age = "Age";
        public static readonly string ContentLength = "ContentLength";
        public static readonly string ContentMd5 = "ContentMd5";
        public static readonly string ContentType = "ContentType";
        public static readonly string ETag = "ETag";
        public static readonly string KeepAlive = "KeepAlive";
        public static readonly string Location = "Location";
        public static readonly string ProxyAuthenticate = "ProxyAuthenticate";
        public static readonly string RetryAfter = "RetryAfter";
        public static readonly string Server = "Server";
        public static readonly string SetCookie = "SetCookie";
        public static readonly string Vary = "Vary";
        public static readonly string WwwAuthenticate = "WwwAuthenticate";

        private static readonly Cache<HttpResponseHeader, string> _headerNames = new Cache<HttpResponseHeader, string>();

        static HttpResponseHeaders()
        {
            _headerNames[HttpResponseHeader.AcceptRanges] = AcceptRanges;
            _headerNames[HttpResponseHeader.Age] = Age;
            _headerNames[HttpResponseHeader.Allow] = Allow;
            _headerNames[HttpResponseHeader.CacheControl] = CacheControl;
            _headerNames[HttpResponseHeader.Connection] = Connection;
            _headerNames[HttpResponseHeader.ContentEncoding] = ContentEncoding;
            _headerNames[HttpResponseHeader.ContentLanguage] = ContentLanguage;
            _headerNames[HttpResponseHeader.ContentLength] = ContentLength;
            _headerNames[HttpResponseHeader.ContentLocation] = ContentLocation;
            _headerNames[HttpResponseHeader.ContentMd5] = ContentMd5;
            _headerNames[HttpResponseHeader.ContentRange] = ContentRange;
            _headerNames[HttpResponseHeader.ContentType] = ContentType;
            _headerNames[HttpResponseHeader.Date] = Date;
            _headerNames[HttpResponseHeader.ETag] = ETag;
            _headerNames[HttpResponseHeader.Expires] = Expires;
            _headerNames[HttpResponseHeader.KeepAlive] = KeepAlive;
            _headerNames[HttpResponseHeader.LastModified] = LastModified;
            _headerNames[HttpResponseHeader.Location] = Location;
            _headerNames[HttpResponseHeader.Pragma] = Pragma;
            _headerNames[HttpResponseHeader.ProxyAuthenticate] = ProxyAuthenticate;
            _headerNames[HttpResponseHeader.RetryAfter] = RetryAfter;
            _headerNames[HttpResponseHeader.Server] = Server;
            _headerNames[HttpResponseHeader.SetCookie] = SetCookie;
            _headerNames[HttpResponseHeader.Trailer] = Trailer;
            _headerNames[HttpResponseHeader.TransferEncoding] = TransferEncoding;
            _headerNames[HttpResponseHeader.Upgrade] = Upgrade;
            _headerNames[HttpResponseHeader.Vary] = Vary;
            _headerNames[HttpResponseHeader.Via] = Via;
            _headerNames[HttpResponseHeader.Warning] = Warning;
            _headerNames[HttpResponseHeader.WwwAuthenticate] = WwwAuthenticate;
        }

        protected HttpResponseHeaders()
        {
        }

        public static string HeaderNameFor(HttpResponseHeader header)
        {
            return _headerNames[header];
        }
    }
}