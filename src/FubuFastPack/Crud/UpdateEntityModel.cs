using System;
using FubuFastPack.Domain;
using FubuFastPack.Extensibility;

namespace FubuFastPack.Crud
{
    public class UpdateEntityModel<T> : IItemRequest where T : DomainEntity
    {
        public Extends<T> ExtendedProperties { get; set; }
        public Guid Id { get; set; }
    }
}