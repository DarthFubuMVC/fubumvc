using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Content
{
    public class ContentRegistryCache : IContentRegistry
    {
        private readonly IEnumerable<IImageUrlResolver> _imageResolvers;
        private readonly Cache<string, string> _urls = new Cache<string,string>();

        public ContentRegistryCache(IEnumerable<IImageUrlResolver> imageResolvers)
        {
            _imageResolvers = imageResolvers;

            _urls.OnMissing = name =>
            {
                var url = _imageResolvers.FirstValue(x => x.UrlFor(name))
                          ?? "~/content/images/" + name;

                return url.ToAbsoluteUrl();
            };
        }


        public string ImageUrl(string name)
        {
            return _urls[name];
        }

        public string CssUrl(string name)
        {
            throw new NotImplementedException();
        }
    }
}