using System;
using FubuFastPack.Domain;

namespace FubuFastPack.Crud.Properties
{
    public class UpdatePropertyModel<T> where T : DomainEntity
    {
        public Guid Id { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
    }
}