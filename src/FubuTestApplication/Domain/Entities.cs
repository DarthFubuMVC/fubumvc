using FluentNHibernate.Mapping;
using FubuFastPack.Domain;

namespace FubuTestApplication.Domain
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

    public class Case : DomainEntity
    {
        public virtual string Identifier { get; set; }
        public virtual string Status { get; set; }
        public virtual string Condition { get; set; }
        public virtual string Title { get; set; }
        public virtual string Priority { get; set; }
        public virtual string CaseType { get; set; }
        public virtual int Number { get; set; }

        public virtual Person Person { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Identifier: {1}", base.ToString(), Identifier);
        }
    }

    public class Person : DomainEntity
    {
        public virtual string Name { get; set; }
    }

    public class PersonMap : ClassMap<Person>
    {
        public PersonMap()
        {
            Table("people");
            Id(x => x.Id);

            Map(x => x.Name);
        }
    }

    public class CaseMap : ClassMap<Case>
    {
        public CaseMap()
        {
            Table("cases");
            Id(x => x.Id);

            Map(x => x.Identifier);
            Map(x => x.Status);
            Map(x => x.Condition);
            Map(x => x.Title);
            Map(x => x.Priority);
            Map(x => x.CaseType);
            Map(x => x.Number);

            References(x => x.Person).Cascade.All();
        }
    }
}