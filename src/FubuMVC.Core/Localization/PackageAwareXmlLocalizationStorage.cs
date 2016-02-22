using FubuMVC.Core.Localization.Basic;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.Localization
{
    public class PackageAwareXmlLocalizationStorage : XmlDirectoryLocalizationStorage
    {
        public PackageAwareXmlLocalizationStorage(IFubuApplicationFiles files) : base(new []{files.RootPath})
        {
        }
    }
}