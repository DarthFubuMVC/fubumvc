using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;


namespace FubuCore.Configuration
{
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