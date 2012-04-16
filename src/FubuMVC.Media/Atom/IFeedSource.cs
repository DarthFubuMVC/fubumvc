using System.Collections.Generic;
using FubuMVC.Core.Projections;

namespace FubuMVC.Media.Atom
{
    public interface IFeedSource<T>
    {
        IEnumerable<IValues<T>> GetValues();
    }
}