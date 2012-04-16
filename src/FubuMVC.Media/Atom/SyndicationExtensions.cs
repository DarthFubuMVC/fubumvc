using System;
using System.ServiceModel.Syndication;
using FubuCore;

namespace FubuMVC.Media.Atom
{
    public static class SyndicationExtensions
    {
        public static TextSyndicationContent ToContent(this string text)
        {
            return new TextSyndicationContent(text);
        }

        public static SyndicationLink ToSyndicationLink(this Link link)
        {
            var syndicationLink = new SyndicationLink(new Uri(link.Url));
            link.Rel.IfNotNull(x => syndicationLink.RelationshipType = x);
            link.Title.IfNotNull(x => syndicationLink.Title = x);
            link.ContentType.IfNotNull(x => syndicationLink.MediaType = x);

            return syndicationLink;
        }
    }
}