using System;
using System.Collections.Generic;

namespace Fubu.Running
{
    public interface IFubuRegistryFinder
    {
        IEnumerable<Type> Find();

    }
}