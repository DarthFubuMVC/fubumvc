using System;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Content
{
    // Tested through StoryTeller
    public class ContentRegistryCache : IContentRegistry
    {
        private readonly ContentFiles _images;
        private ContentFiles _scripts;
        private ContentFiles _styles;

        public ContentRegistryCache(IContentFolderService folders)
        {
            _images = new ContentFiles(folders, ContentType.images);
            _scripts = new ContentFiles(folders, ContentType.scripts);
            _styles = new ContentFiles(folders, ContentType.styles);
        }


        public string ImageUrl(string name)
        {
            return _images.File(name).ToAbsoluteUrl();
        }

        public string CssUrl(string name, bool optional)
        {
            var file = _styles.File(name, optional);
            return file != null ? file.ToAbsoluteUrl() : null;
        }

        public string ScriptUrl(string name, bool optional)
        {
            var file = _scripts.File(name, optional);
            return file != null ? file.ToAbsoluteUrl() : null;
        }
    }

    public class ContentFiles
    {
        private readonly ContentType _contentType;
        private readonly IContentFolderService _service;
        private readonly Cache<Tuple<string, bool>, string> _urls = new Cache<Tuple<string, bool>, string>();

        public ContentFiles(IContentFolderService service, ContentType contentType)
        {
            _service = service;
            _contentType = contentType;


            /*
             * You might be wondering, "why is it doing this?"
             * The goal of the below is to say:
             * 1.) Use the url to the main application's content/{type} if the file exists there
             * 2.) If not, try to find it in a package folder.  If found, pull it from the package folder
             * 3.) Finally, try the url to the main application and just hope.
             * 
             * This implementation isn't smart enough to deal with "appended" virtual directory paths
             * 
             */
            _urls.OnMissing = (args) =>
            {
                var filename = args.Item1;
                var optional = args.Item2;
                if (_service.ExistsInApplicationDirectory(_contentType, filename))
                {
                    return "~/content/{0}/{1}".ToFormat(_contentType, filename.TrimStart('/'));
                }

                if (_service.FileExists(_contentType, filename))
                {
                    return "~/_content/{0}/{1}".ToFormat(_contentType, filename.TrimStart('/'));
                }

                if (optional) return null;
                return "~/content/{0}/{1}".ToFormat(_contentType, filename.TrimStart('/'));
            };
        }

        public string File(string fileName, bool optional = false)
        {
            return _urls[new Tuple<string, bool>(fileName, optional)];
        }
    }
}