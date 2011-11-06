using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Packaging;
using FubuMVC.StructureMap;
using StructureMap;
using System.Linq;

namespace Serenity.Jasmine
{
    public interface ISerenityJasmineApplication
    {
        void AddContentFolder(string contentFolder);
    }

    public class SerenityJasmineApplication : IApplicationSource, IPackageLoader, ISerenityJasmineApplication
    {
        private readonly IList<string> _contentFolders = new List<string>();

        public void AddContentFolder(string contentFolder)
        {
            _contentFolders.Add(contentFolder);
        }

        public FubuApplication BuildApplication()
        {
            return FubuApplication
                .For<SerenityJasmineRegistry>()
                .StructureMap(new Container())
                .Packages(x =>
                {
                    x.Assembly(GetType().Assembly);
                    x.Loader(this);
                });
        }

        public string Name
        {
            get { return "Serenity Jasmine Runner"; }
        }

        public IEnumerable<IPackageInfo> Load(IPackageLog log)
        {
            return _contentFolders.Select(x =>
            {
                log.Trace("Loading content package from " + x);
                return new ContentOnlyPackageInfo(x);
            });
        }
    }
}