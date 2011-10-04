using System;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Resources.Media
{
    public interface IMediaReaderNode
    {
        Type InputType { get; }
        ObjectDef ToObjectDef();
    }
}