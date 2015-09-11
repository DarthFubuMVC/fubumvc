using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Services
{
    public interface IApplicationLoader
    {
        IDisposable Load(Dictionary<string, string> properties);
    }
}