using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Rest.Media
{
    public interface IMediaWriterNode : IContainerModel
    {
        Type InputType { get; }
    }
}