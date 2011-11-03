using System;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public class AssetFileDataMother
    {
        private readonly Action<AssetPath, AssetFile> _callback;
        private readonly Cache<string, AssetFile> _files = new Cache<string, AssetFile>();

        public AssetFileDataMother(Action<AssetPath, AssetFile> callback)
        {
            _callback = callback;
        }

        public AssetFile this[string key]
        {
            get { return _files[key]; }
        }

        public void LoadAssets(string text)
        {
            text.ReadLines(LoadAsset);
        }

        //key=type/name
        private void LoadAsset(string line)
        {
            if (line.Trim().IsEmpty()) return;

            if (!line.Contains("="))
            {
                line = Guid.NewGuid().ToString() + "=" + line;
            }

            var parts = line.Split('=');
            var key = parts.First();
            var stringPath = parts.Last();
            var @override = stringPath.EndsWith("!override");
            if (@override)
            {
                stringPath = stringPath.Replace("!override", "");
            }

            var path = new AssetPath(stringPath);
            if (path.Package.IsEmpty())
            {
                path = new AssetPath("application:" + stringPath);
            }

            var file = new AssetFile(path.Name){
                Override = @override
            };


            _files[key] = file;

            _callback(path, file);
        }
    }
}