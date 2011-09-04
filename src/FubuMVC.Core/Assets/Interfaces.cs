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


    public interface IAssetRegistration
    {
        void Alias(string name, string alias);
        void Dependency(string dependent, string dependency);
        void Extension(string extender, string @base);
        void AddToSet(string setName, string name);
        void Preceeding(string beforeName, string afterName);
    }


    public class RecordingAssetRegistration : IAssetRegistration
    {
        private readonly IList<Action<IAssetRegistration>> _registrations = new List<Action<IAssetRegistration>>();

        public void Replay(IAssetRegistration registration)
        {
            throw new NotImplementedException();
        }

        public void Alias(string name, string alias)
        {
            throw new NotImplementedException();
        }

        public void Dependency(string dependent, string dependency)
        {
            throw new NotImplementedException();
        }

        public void Extension(string extender, string @base)
        {
            throw new NotImplementedException();
        }

        public void AddToSet(string setName, string name)
        {
            throw new NotImplementedException();
        }

        public void Preceeding(string beforeName, string afterName)
        {
            throw new NotImplementedException();
        }
    }

}