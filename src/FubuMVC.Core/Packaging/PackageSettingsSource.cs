using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Configuration;

namespace FubuMVC.Core.Packaging
{
    public class PackageSettingsSource : ISettingsSource
    {
        public static readonly string FILE = "packageSettings.config";
        private readonly IPackageFiles _files;

        public static void WriteToDirectory(SettingsData data, string directory)
        {
            data.Category = SettingCategory.package;
            XmlSettingsParser.Write(data, directory.AppendPath(FILE));
        }

        public PackageSettingsSource(IPackageFiles files)
        {
            _files = files;
        }

        public IEnumerable<SettingsData> FindSettingData()
        {
            FileSet fileSet = GetFileSet();

            return _files.FindFiles(fileSet).Select(file =>
            {
                var data = XmlSettingsParser.Parse((string) file);
                data.Category = SettingCategory.package;

                return data;
            });
        }

        public static FileSet GetFileSet()
        {
            return new FileSet(){
                DeepSearch = true,
                Include = "config/*.config;" + FILE
            };
        }
    }
}