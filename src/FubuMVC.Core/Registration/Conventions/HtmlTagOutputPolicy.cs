using System.Collections.Generic;
using System.Linq;
using HtmlTags;
using FubuCore;

namespace FubuMVC.Core.Registration.Conventions
{
    public class HtmlTagOutputPolicy : Policy
    {
        public HtmlTagOutputPolicy()
        {
            Where.ResourceTypeImplements<HtmlTag>().Or.ResourceTypeImplements<HtmlDocument>();
            Conneg.AddHtml();
        }
    }
}