using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Projections;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest
{

    public class Link
    {
        public string Url { get; set; }
        public string Rel { get; set; }
        public string Title { get; set; }
    }




}