using System;
using FubuCore;

namespace FubuMVC.Core.Resources
{
    public class Link
    {
        public Link(string url)
        {
            throw new NotImplementedException("REDO");
            //Url = url.ToAbsoluteUrl();
        }

        public string Url { get; private set; }
        public string Rel { get; set; }
        public string Title { get; set; }

        public string ContentType { get; set; }
    }
}