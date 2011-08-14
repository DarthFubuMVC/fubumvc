using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FubuMVC.Core.Assets.Files;
using FubuCore;

namespace FubuMVC.Core.Assets.Combination
{
    public class AssetTagPlan
    {
        private readonly string _mimeType;
        private readonly IList<IAssetTagSubject> _subjects = new List<IAssetTagSubject>();

        public AssetTagPlan(string mimeType, IEnumerable<AssetFile> files)
        {
            _mimeType = mimeType;

            _subjects.AddRange(files);
        }
    }
    

    public class ScriptFileCombination : AssetFileCombination
    {
        public ScriptFileCombination(IEnumerable<AssetFile> files) : base(null, ".js", files)
        {
        }

        public override AssetFolder? Folder
        {
            get { return AssetFolder.scripts; }
        }

        public override string MimeType
        {
            get { return MimeTypeProvider.JAVASCRIPT; }
        }
    }

    public class StyleFileCombination : AssetFileCombination
    {
        public StyleFileCombination(string folder, IEnumerable<AssetFile> files) : base(folder, ".css", files)
        {
        }

        public override AssetFolder? Folder
        {
            get { return AssetFolder.styles; }
        }

        public override string MimeType
        {
            get { return MimeTypeProvider.CSS; }
        }
    }


    public abstract class AssetFileCombination : IAssetTagSubject
    {
        private readonly string _name;
        private readonly IEnumerable<AssetFile> _files;

        public AssetFileCombination(string folder, string extension, IEnumerable<AssetFile> files)
        {
            var basicName = getCombinedName(files) + extension;
            _name = folder.IsNotEmpty() ? folder + "/" + basicName : basicName;

            _files = files;
        }

        private static string getCombinedName(IEnumerable<AssetFile> rawFiles)
        {
            var name = rawFiles.Select(x => x.Name.ToLowerInvariant()).OrderBy(x => x).Join("*");
            return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(name)).Select(b => b.ToString("x2")).Join("");
        }

        public string Name
        {
            get { return _name; }
        }

        public abstract AssetFolder? Folder { get; }
        public abstract string MimeType { get; }

        public IEnumerable<AssetFile> Files
        {
            get { return _files; }
        }
    }

  
}