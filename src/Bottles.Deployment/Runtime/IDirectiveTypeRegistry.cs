using System;

namespace Bottles.Deployment.Runtime
{
    public interface IDirectiveTypeRegistry
    {
        Type DirectiveTypeFor(string name);
    }
}