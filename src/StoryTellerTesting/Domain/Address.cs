using FluentNHibernate.Mapping;
using FubuFastPack.Domain;

namespace IntegrationTesting.Domain
{
    public class Address : DomainEntity
    {
        public virtual string Line1 { get; set; }
        public virtual string Line2 { get; set; }
    
        
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

    public class Site : DomainEntity
    {
        public virtual string Name { get; set; }
    }

    public class SiteMap : ClassMap<Site>
    {
        public SiteMap()
        {
            Table("site");
            Id(x => x.Id);
            Map(x => x.Name);
        }
    }
}