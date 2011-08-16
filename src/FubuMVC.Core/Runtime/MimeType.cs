using System;
using System.Collections.Generic;
using System.Net.Mime;
using FubuCore.Util;
using System.Linq;

namespace FubuMVC.Core.Runtime
{
    public class MimeType
    {
        // This *must* go before the code below
        private static readonly Cache<string, MimeType> _mimeTypes =
            new Cache<string, MimeType>(key => new MimeType(key));

        public static readonly MimeType Html = New(MediaTypeNames.Text.Html, ".htm", ".html");
        public static readonly MimeType Json = New("application/json");
        public static readonly MimeType Text = New(MediaTypeNames.Text.Plain, ".txt");
        public static readonly MimeType Javascript = New("application/javascript", ".js");
        public static readonly MimeType Css = New("text/css", ".css");

        public static readonly MimeType Gif = New("image/gif", ".gif");
        public static readonly MimeType Png = New("image/png", ".png");
        public static readonly MimeType Jpg = New("image/jpeg", ".jpg", ".jpeg");
        public static readonly MimeType Bmp = New("image/bmp", ".bmp", ".bm");



        private readonly IList<string> _extensions = new List<string>();
        private readonly string _mimeType;

        private MimeType(string mimeType)
        {
            _mimeType = mimeType;
        }

        public string Value
        {
            get { return _mimeType; }
        }

        public static MimeType New(string mimeTypeValue, params string[] extensions)
        {
            var mimeType = new MimeType(mimeTypeValue);
            extensions.Each(mimeType.AddExtension);
            _mimeTypes[mimeTypeValue] = mimeType;

            return mimeType;
        }

        public void AddExtension(string extension)
        {
            _extensions.Add(extension);
        }

        public override string ToString()
        {
            return _mimeType;
        }

        public static IEnumerable<MimeType> All()
        {
            return _mimeTypes.GetAll();
        }

        public static MimeType FindMimeType(string mimeTypeValue)
        {
            return _mimeTypes[mimeTypeValue];
        }

        public bool HasExtension(string extension)
        {
            return _extensions.Contains(extension);
        }

        public string DefaultExtension()
        {
            return _extensions.FirstOrDefault();
        }
    }
}