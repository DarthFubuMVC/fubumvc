using System;
using FubuLocalization.Basic;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Localization
{
    public class PackageAwareXmlLocalizationStorage : XmlDirectoryLocalizationStorage
    {
        public PackageAwareXmlLocalizationStorage(IFubuApplicationFiles files) : base(new []{files.GetApplicationPath()})
        {
        }
    }
}