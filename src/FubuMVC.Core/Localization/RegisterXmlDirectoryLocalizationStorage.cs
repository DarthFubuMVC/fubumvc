using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuLocalization.Basic;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Localization
{
    public class RegisterXmlDirectoryLocalizationStorage : IActivator
    {
        private readonly IContainerFacility _facility;

        public RegisterXmlDirectoryLocalizationStorage(IContainerFacility facility)
        {
            _facility = facility;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            var list = new List<string>();
            log.Trace("Setting up the {0} with directories", typeof(XmlDirectoryLocalizationStorage).Name);
            
            list.Add(FubuMvcPackageFacility.GetApplicationPath());

            packages.Each(pak => pak.ForFolder(BottleFiles.WebContentFolder, list.Add));

            var storage = new XmlDirectoryLocalizationStorage(list);

            _facility.Register(typeof(ILocalizationStorage), ObjectDef.ForValue(storage));
        }
    }
}