using System;
using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public interface IAsset
    {
        string Name { get; }
        bool Matches(string key);
        void AddAlias(string alias);

        // TODO -- why are both here?
        IEnumerable<IAssetDependency> AllScripts();
        IEnumerable<IAsset> Dependencies();
        void AddDependency(IAsset asset);
    }

    public interface IAssetDependency : IAsset, IComparable<IAssetDependency>
    {
        bool MustBeAfter(IAssetDependency assetDependency);
        void MustBePreceededBy(IAssetDependency assetDependency);
        void AddExtension(IAssetDependency extender);

        bool IsFirstRank();
    }


    public interface IAssetRegistration
    {
        void Alias(string name, string alias);
        void Dependency(string dependent, string dependency);
        void Extension(string extender, string @base);
        void AddToSet(string setName, string name);
        void Preceeding(string beforeName, string afterName);
    }

    public interface IScriptTagWriter
    {
        IEnumerable<HtmlTag> Write(IEnumerable<string> scripts);
    }


}