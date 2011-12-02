using System;
using System.Collections.Generic;

namespace Fubu.Applications
{
    public interface IApplicationSourceTypeFinder
    {
        IEnumerable<Type> FindApplicationSourceTypes();
    }
}