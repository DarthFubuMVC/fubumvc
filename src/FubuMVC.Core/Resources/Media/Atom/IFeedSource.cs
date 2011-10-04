using System.Collections.Generic;

namespace FubuMVC.Core.Resources.Media.Atom
{
    public interface IFeedSource<T>
    {
        IEnumerable<IValues<T>> GetValues();
    }
}