using System;

namespace FubuMVC.Core.Rest
{
    public class Link
    {
        public string Url { get; set; }
        public string Rel { get; set; }
        public string Title { get; set; }

        public string Mimetype { get; set; }
    }
}