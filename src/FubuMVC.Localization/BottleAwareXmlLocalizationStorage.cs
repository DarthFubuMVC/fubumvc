using System;
using FubuLocalization.Basic;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Localization
{
    public class BottleAwareXmlLocalizationStorage : XmlDirectoryLocalizationStorage
    {
        public BottleAwareXmlLocalizationStorage(IFubuApplicationFiles files) : base(new []{files.GetApplicationPath()})
        {
        }
    }
}