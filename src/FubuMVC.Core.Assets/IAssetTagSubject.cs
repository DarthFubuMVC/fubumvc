using System;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets
{
    public interface IAssetTagSubject
    {
        string Name { get; }
        AssetFolder Folder { get; }
        MimeType MimeType { get; }
    }
}