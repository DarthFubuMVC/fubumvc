using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Caching
{
    public interface IVaryBy
    {
        IDictionary<string, string> Values();
    }
}