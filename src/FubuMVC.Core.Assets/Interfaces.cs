using System;
using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public interface IRequestedAsset
    {
        string Name { get; }
        bool Matches(string key);
        void AddAlias(string alias);

        // TODO -- why are both here?
        IEnumerable<IFileDependency> AllFileDependencies();
        IEnumerable<IRequestedAsset> Dependencies();

        void AddDependency(IRequestedAsset asset);
    }

    public interface IFileDependency : IRequestedAsset, IComparable<IFileDependency>
    {
        bool MustBeAfter(IFileDependency fileDependency);
        void MustBePreceededBy(IFileDependency fileDependency);
        void AddExtension(IFileDependency extender);

        bool IsFirstRank();
    }
}