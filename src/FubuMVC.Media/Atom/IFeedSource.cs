using System.Collections.Generic;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media.Atom
{
    public interface IFeedSource<T>
    {
        IEnumerable<IValues<T>> GetValues();
    }
}