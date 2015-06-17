using System.Linq;
using FubuLocalization.Basic;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Localization
{
    public class BottleAwareXmlLocalizationStorage : XmlDirectoryLocalizationStorage
    {
        public BottleAwareXmlLocalizationStorage(IFubuApplicationFiles files) : base(files.AllFolders.Select(x => x.Path))
        {
        }
    }
}