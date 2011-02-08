using System;

namespace FubuMVC.Core.Urls
{
    public interface IUrlTemplatePattern
    {
        string Start { get; }
        string End { get; }
    }

    public class NulloUrlTemplate : IUrlTemplatePattern
    {
        public string Start
        {
            get { return "{"; }
        }

        public string End
        {
            get { return "}"; }
        }
    }

    public class JQueryUrlTemplate : IUrlTemplatePattern
    {
        public string Start
        {
            get { return "${"; }
        }

        public string End
        {
            get { return "}"; }
        }
    }
}