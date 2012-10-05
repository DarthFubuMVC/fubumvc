using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FubuCore;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{

    public abstract class AssetFileCombination : IAssetTagSubject
    {
        private readonly IEnumerable<AssetFile> _files;

        protected AssetFileCombination(string folder, MimeType mimeType, IEnumerable<AssetFile> files)
        {
            MimeType = mimeType;
            var basicName = GetCombinedName(files) + mimeType.DefaultExtension();
            Name = folder.IsNotEmpty() ? folder + "/" + basicName : basicName;

            _files = files;
        }

        public static string GetCombinedName(IEnumerable<AssetFile> rawFiles)
        {
            var name = rawFiles.Select(x => x.Name.ToLowerInvariant()).OrderBy(x => x).Join("*");
            return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(name)).Select(b => b.ToString("x2")).Join("");
        }

        public string Name
        {
            get; set;
        }

        public abstract AssetFolder Folder { get; }
        public MimeType MimeType { get; private set; }

        public IEnumerable<AssetFile> Files
        {
            get { return _files; }
        }

        public AssetFile FileAt(int index)
        {
            return _files.ElementAt(index);
        }

        public int Length
        {
            get
            {
                return _files.Count();
            }
        }
    }
}