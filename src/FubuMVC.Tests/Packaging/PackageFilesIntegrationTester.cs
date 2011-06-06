using System;
using System.IO;
using FubuCore;
using FubuCore.Configuration;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using System.Collections.Generic;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class PackageFilesIntegrationTester
    {
        private PackageFilesCache theCache;

        private SettingsData newData()
        {
            var data = new SettingsData();
            data[Guid.NewGuid().ToString()] = Guid.NewGuid().ToString();

            return data;
        }

        private void saveData(SettingsData data, string directory, string name)
        {
            var file = FileSystem.Combine("geonosis", directory, "config", name + ".config");
            new FileSystem().CreateDirectory("geonosis", directory);
            new FileSystem().CreateDirectory("geonosis", directory, "config");

            XmlSettingsParser.Write(data, file);
        }

        [SetUp]
        public void SetUp()
        {
            var system = new FileSystem();
            system.DeleteDirectory("geonosis");
            system.CreateDirectory("geonosis");

            var data1 = newData();
            var data2 = newData();
            var data3 = newData();
            var data4 = newData();
            var data5 = newData();
            var data6 = newData();
            var data7 = newData();


            

            saveData(data1, "a", "a1");
            saveData(data2, "a", "a2");
            saveData(data3, "b", "b3");
            saveData(data4, "b", "b4");
            saveData(data5, "c", "c5");
            saveData(data6, "c", "c6");
            saveData(data7, "c", "c7");

            var data8 = newData();
            data8["pak1"] = "pak1-value";
            PackageSettingsSource.WriteToDirectory(data8, "geonosis".AppendPath("a"));

            theCache = new PackageFilesCache();
            theCache.AddDirectory(FileSystem.Combine("geonosis", "a"));
            theCache.AddDirectory(FileSystem.Combine("geonosis", "b"));
            theCache.AddDirectory(FileSystem.Combine("geonosis", "c"));

            
        }

        [Test]
        public void find_files()
        {
            var fileSpec = PackageSettingsSource.GetFileSet();
            theCache.FindFiles(fileSpec).Select(x => Path.GetFileNameWithoutExtension(x)).OrderBy(x => x)
                .ShouldHaveTheSameElementsAs("a1", "a2", "b3", "b4", "c5", "c6", "c7", "packageSettings");
        }

        [Test]
        public void use_package_settings_source()
        {
            var source = new PackageSettingsSource(theCache);
            var allData = source.FindSettingData();
            allData.Each(x => x.Category.ShouldEqual(SettingCategory.package));

            allData.Count().ShouldEqual(8);
        }
    }
}