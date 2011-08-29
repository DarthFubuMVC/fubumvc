using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Http
{
    public delegate void ProcessContentAction(string contents, IEnumerable<AssetFile> files);
}