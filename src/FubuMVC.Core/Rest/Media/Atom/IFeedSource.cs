using System.Collections.Generic;

namespace FubuMVC.Core.Rest.Media.Atom
{
    public interface IFeedSource<T>
    {
        IEnumerable<IValues<T>> GetValues();
    }
}