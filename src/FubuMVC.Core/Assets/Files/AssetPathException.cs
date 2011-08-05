using System;
using System.Runtime.Serialization;
using FubuCore;

namespace FubuMVC.Core.Assets.Files
{
    [Serializable]
    public class AssetPathException : Exception
    {
        public AssetPathException(string path) : base(@"Asset path {0} is invalid.  Options are:
[name]
scripts/[name]
styles/[name]
images/[name]
[package name]:[name]
[package name]:scripts/[name]
[package name]:styles/[name]
[package name]:images/[name]
                    
                    ".ToFormat(path))
        {
        }


        protected AssetPathException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}