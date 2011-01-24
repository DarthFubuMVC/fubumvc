using System;

namespace FubuMVC.Core.UI.Scripts
{
    public interface IScript : IScriptObject, IComparable<IScript>
    {
        bool MustBeAfter(IScript script);
        void MustBePreceededBy(IScript script);
        void AddExtension(IScript extender);

        bool IsFirstRank();
    }
}