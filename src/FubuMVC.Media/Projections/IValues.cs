using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Media.Projections
{
    public interface IValues<T>
    {
        T Subject { get; }
        object ValueFor(Accessor accessor);
        
    }

}