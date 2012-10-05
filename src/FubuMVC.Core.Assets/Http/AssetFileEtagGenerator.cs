using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Resources.Etags;

namespace FubuMVC.Core.Assets.Http
{
    public class AssetFileEtagGenerator : IETagGenerator<IEnumerable<AssetFile>>
    {
        public string Create(IEnumerable<AssetFile> target)
        {
            return target.Select(x => x.FullPath).HashByModifiedDate();
        }
    }
}