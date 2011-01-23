using System;

namespace FubuMVC.Core.UI.Scripts
{
    public interface IScript : IScriptObject, IComparable<IScript>
    {
        bool DependsOn(IScript script);

        void AddExtension(IScript extender);

        bool HasDependencies();
    }
}