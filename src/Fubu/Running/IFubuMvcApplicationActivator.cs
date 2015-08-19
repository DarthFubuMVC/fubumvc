using System;

namespace Fubu.Running
{
    public interface IFubuMvcApplicationActivator
    {
        void Initialize(Type applicationType, StartApplication message);
        void ShutDown();
        void Recycle();
    }
}