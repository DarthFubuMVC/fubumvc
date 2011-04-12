using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;

namespace FubuCore.Configuration
{
    public enum SettingCategory
    {
        environment,
        package,
        core
    }

    public interface ISettingsData
    {
        string Description { get; }
        SettingCategory Category { get; }

        bool Has(string key);
        string Get(string key);
        IEnumerable<string> AllKeys { get; }
    }


    public interface ISettingsSource
    {
        IEnumerable<ISettingsData> FindSettingData();
    }

    // Assume the same file structure as an appSettings file,
    // with the caveat that the first node can have a "category" attribute
    public class FolderAppSettingsXmlSource : ISettingsSource
    {
        private readonly string _folder;

        public FolderAppSettingsXmlSource(string folder)
        {
            _folder = folder;
        }

        public IEnumerable<ISettingsData> FindSettingData()
        {
            return Directory.GetFiles(_folder, "*.config").Select(file =>
            {
                var document = new XmlDocument();
                document.Load(file);

                return new XmlSettingsData(document.DocumentElement){
                    Description = file
                };
            });
        }
    }
}