using System;
using FubuCore.Dates;

namespace FubuPersistence
{
    // All this means is that it has an Id, nothing else
    public interface IEntity
    {
        Guid Id { get; set; }
    }

    public interface ISoftDeletedEntity : IEntity
    {
        Milestone Deleted { get; set; }
    }

    
}