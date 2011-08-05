using System;
using System.IO;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Assets.Files
{
    [TestFixture]
    public class PackageAssetsTester
    {
        private PackageAssets thePackageAssets;
        private Cache<string, AssetFile> theFiles;

        [SetUp]
        public void SetUp()
        {
            thePackageAssets = new PackageAssets();
            theFiles = new Cache<string, AssetFile>();
        }

        private void loadAssets(string text)
        {
            var reader = new StringReader(text);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                loadAsset(line);   
            }
        }

        //key=type/name
        private void loadAsset(string line)
        {
            var parts = line.Split('=');
            var key = parts.First();

            
        }
    }
}