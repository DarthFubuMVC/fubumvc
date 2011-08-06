using System;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Tests.Assets.Files
{
    public class AssetFileDataMother
    {
        private readonly Action<AssetPath, AssetFile> _callback;
        private readonly Cache<string, AssetFile> _files = new Cache<string, AssetFile>();

        public AssetFileDataMother(Action<AssetPath, AssetFile> callback)
        {
            _callback = callback;
        }

        public void LoadAssets(string text)
        {
            var reader = new StringReader(text);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                LoadAsset(line);
            }
        }

        //key=type/name
        private void LoadAsset(string line)
        {
            if (line.Trim().IsEmpty()) return;

            var parts = line.Split('=');
            var key = parts.First();
            var path = new AssetPath(parts.Last());

            var file = new AssetFile()
                       {
                           Name = path.Name
                       };

            _files[key] = file;

            _callback(path, file);
        }

        public AssetFile this[string key]
        {
            get
            {
                return _files[key];
            }
        }
    
    }
}