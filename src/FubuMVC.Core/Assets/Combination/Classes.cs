using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FubuMVC.Core.Assets.Files;
using FubuCore;

namespace FubuMVC.Core.Assets.Combination
{
    //public class AssetFileRequirements
    //{
    //    private readonly IList<>

    //    public AssetFileRequirements(IEnumerable<AssetFile> files)
    //    {
    //    }
    //}
    


    public class AssetFileCombination
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

        public IEnumerable<AssetFile> Files
        {
            get { return _files; }
        }
    }

  
}