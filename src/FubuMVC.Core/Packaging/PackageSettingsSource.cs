using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Configuration;

namespace FubuMVC.Core.Packaging
{
    public class PackageSettingsSource : ISettingsSource
    {
        private readonly IPackageFiles _files;

        public PackageSettingsSource(IPackageFiles files)
        {
            _files = files;
        }

        public IEnumerable<SettingsData> FindSettingData()
        {
            var fileSet = new FileSet(){
                DeepSearch = true,
                Include = "config/*.config"
            };

            return _files.FindFiles(fileSet).Select(file =>
            {
                var data = XmlSettingsParser.Parse((string) file);
                data.Category = SettingCategory.package;

                return data;
            });
        }
    }
}