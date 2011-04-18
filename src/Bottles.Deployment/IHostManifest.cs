using System;
using System.Collections.Generic;

namespace Bottles.Deployment
{
    public interface IHostManifest
    {
        T GetDirective<T>() where T : class, new();
        IDirective GetDirective(Type directiveType);

        string Name { get; }
    }
}