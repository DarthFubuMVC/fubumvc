using System;
using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public interface IScriptObject //: IEnumerable<IScript>
    {
        string Name { get; }
        bool Matches(string key);
        void AddAlias(string alias);

        IEnumerable<IScript> AllScripts();
        IEnumerable<IScriptObject> Dependencies();
        void AddDependency(IScriptObject scriptObject);
    }

    public interface IScript : IScriptObject, IComparable<IScript>
    {
        bool MustBeAfter(IScript script);
        void MustBePreceededBy(IScript script);
        void AddExtension(IScript extender);

        bool IsFirstRank();
    }


    public interface IScriptRegistration
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