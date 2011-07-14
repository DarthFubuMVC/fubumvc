using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Content;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.UI.Scripts;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    public class ContentFileCombinerPackage : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services(x =>
            {
                x.ReplaceService<IContentFileCombiner, ContentFileCombiner>();
                x.ReplaceService<IScriptTagWriter, CombiningScriptTagWriter>();
                x.ReplaceService<ICssLinkTagWriter, CombiningCssLinkTagWriter>();
            });
        }
    }

    public static class ContentFileCombinerExtensions
    {
        [Obsolete("This is going to be replaced before the 1.0 release")]
        public static void CombineScriptAndCssFiles(this FubuRegistry registry)
        {
            new ContentFileCombinerPackage().Configure(registry);
        }
    }

    public class CombiningScriptTagWriter : IScriptTagWriter
    {
        private readonly IContentFolderService _folderService;
        private readonly IContentFileCombiner _fileCombiner;

        public CombiningScriptTagWriter(IContentFolderService folderService, IContentFileCombiner fileCombiner)
        {
            _folderService = folderService;
            _fileCombiner = fileCombiner;
            _folderService.RegisterDirectory(FileSystem.Combine(FubuMvcPackageFacility.GetApplicationPath(), "content"));
        }

        public IEnumerable<HtmlTag> Write(IEnumerable<string> scripts)
        {
            var rawFiles = scripts.Select(script => _folderService.FileNameFor(ContentType.scripts, script)).Where(x => x != null).ToArray();
            var combinedName = _fileCombiner.GenerateCombinedFile(rawFiles, "; // src: {0}");
            if (combinedName == null) { return Enumerable.Empty<HtmlTag>(); }
            var scriptUrl = "~/content/{0}".ToFormat(combinedName).ToAbsoluteUrl();
            return new[] {new HtmlTag("script").Attr("src", scriptUrl).Attr("type", "text/javascript")};
        }
    }

    public class CombiningCssLinkTagWriter : ICssLinkTagWriter
    {
        private readonly IContentFolderService _folderService;
        private readonly IContentFileCombiner _fileCombiner;
        private readonly CssLinkTagWriter _singleFileWriter;

        public CombiningCssLinkTagWriter(IContentFolderService folderService, IContentFileCombiner fileCombiner, CssLinkTagWriter singleFileWriter)
        {
            _folderService = folderService;
            _fileCombiner = fileCombiner;
            _singleFileWriter = singleFileWriter;
            _folderService.RegisterDirectory(FileSystem.Combine(FubuMvcPackageFacility.GetApplicationPath(), "content"));
        }

        public IEnumerable<HtmlTag> Write(IEnumerable<string> stylesheets)
        {
            // if only 1 file, use single file writer... or should it write the file (w/its real name) anyway, to get it out of _content?
            if (!stylesheets.Skip(1).Any()) return _singleFileWriter.Write(stylesheets);
            // TODO: keep track of files that are combined, for this request. if it is asked for again, do not render anything
            var rawFiles = stylesheets.Select(file => _folderService.FileNameFor(ContentType.styles, file)).Where(x => x != null).ToArray();
            var combinedName = _fileCombiner.GenerateCombinedFile(rawFiles, "/* src: {0} */");
            if (combinedName == null) { return Enumerable.Empty<HtmlTag>(); }
            var url = "~/content/{0}".ToFormat(combinedName).ToAbsoluteUrl();
            return new[] { new HtmlTag("link").Attr("href", url).Attr("rel", "stylesheet").Attr("type", "text/css") };
        }

        public IEnumerable<HtmlTag> WriteIfExists(IEnumerable<string> stylesheets)
        {
            return Write(stylesheets);
        }
    }
}