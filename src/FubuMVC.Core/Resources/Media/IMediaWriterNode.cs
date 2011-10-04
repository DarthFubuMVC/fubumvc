using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Resources.Media
{
    public interface IMediaWriterNode : IContainerModel
    {
        Type InputType { get; }
    }
}