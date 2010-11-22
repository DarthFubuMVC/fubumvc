using FluentNHibernate.Mapping;
using FubuFastPack.Domain;

namespace IntegrationTesting.Domain
{
    public class Address : DomainEntity
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
    
        
    }

    public class AddressMap : ClassMap<Address>
    {
        public AddressMap()
        {
            Table("address");
            Id(x => x.Id);
            Map(x => x.Line1);
            Map(x => x.Line2);
        }
    }
}