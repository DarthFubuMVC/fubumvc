using System;
using System.Collections.Generic;

namespace Fubu.Running
{
    public interface IApplicationSourceFinder
    {
        IEnumerable<Type> Find();

    }
}