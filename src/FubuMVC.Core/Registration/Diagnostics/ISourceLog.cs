using System;
using System.Collections.Generic;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public interface ISourceLog
    {
        ProvenanceChain ProvenanceChain { get; }
        Guid Id { get; }
        Description Description { get; }
        IList<NodeEvent> Events { get; }
    }
}