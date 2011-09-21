using System;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Rest.Conneg
{
    public interface IMediaReaderNode
    {
        Type InputType { get;}
        ObjectDef ToObjectDef();
    }
}