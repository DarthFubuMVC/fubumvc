using System;

namespace Fubu.Running
{
    public interface IFubuMvcApplicationActivator
    {
        void Initialize(Type applicationType, int port, string physicalPath, string htmlHeadInjectedText);
        void ShutDown();
        void Recycle();
    }
}