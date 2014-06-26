using System;

namespace Fubu.Running
{
    public interface IFubuMvcApplicationActivator
    {
        void Initialize(Type applicationType, int port, string physicalPath);
        void ShutDown();
        void Recycle();
        void GenerateTemplates();
    }
}