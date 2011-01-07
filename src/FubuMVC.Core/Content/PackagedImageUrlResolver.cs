using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Content
{
    public class PackagedImageUrlResolver : IImageUrlResolver
    {
        private readonly IContentFolderService _folders;

        public PackagedImageUrlResolver(IContentFolderService folders)
        {
            _folders = folders;
        }

        public string UrlFor(string name)
        {
            return _folders.FileExists(ContentType.images, name) ? "~/_images/" + name.TrimStart('/') : null;
        }
    }
}