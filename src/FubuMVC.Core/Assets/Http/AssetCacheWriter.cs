using System;
using System.IO;
using System.Net;
using FubuCore;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Http
{
    public class AssetCacheWriter : IAssetCacheWriter
    {
        private readonly IOutputWriter _output;

        public AssetCacheWriter(IOutputWriter output)
        {
            _output = output;
            LastModifiedLambda = File.GetLastWriteTimeUtc;
        }

        // Here for unit testing
        public Func<string, DateTime> LastModifiedLambda { get; set; }

        public void Write(AssetFile file)
        {
            if (!FubuMode.InDevelopment())
            {
                WriteCachingHeaders(file);
            }
        }

        public virtual void WriteCachingHeaders(AssetFile file)
        {
            var lastMod = LastModifiedLambda(file.FullPath);

            _output.AppendHeader(HttpResponseHeader.LastModified, lastMod.ToString("R"));
            //setting max-age to 8 hours
            _output.AppendHeader(HttpResponseHeader.CacheControl, "{0}={1}".ToFormat("private, max-age", 24 * 60 * 60));
        }
    }
}