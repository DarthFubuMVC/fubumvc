using System;

namespace FubuMVC.Core.UI.Scripts
{
    public interface IScript : IScriptObject, IComparable<IScript>
    {
        bool DependsOn(IScript script);

        void OrderedAfter(IScript script);
        void OrderedBefore(IScript script);
        void AddExtension(IScript extender);

        bool HasDependencies();
    }
}