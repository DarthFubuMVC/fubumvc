using System;
using System.Xml.Serialization;
using FubuCore;

namespace FubuFastPack.Domain
{
    [Serializable]
    public class DomainEntity : Entity //, IValidated
    {
        public virtual string TypeName
        {
            get
            {
                return this.GetTrueType().Name;
            }
        }

        public virtual string Path
        {
            get
            {
                return BuildPath(this.GetTrueType(), Id);
            }
        }

        public virtual string EntityDescription
        {
            get
            {
                return string.Empty;
            }
        }

        [XmlIgnore] //Ignore for DataLoader.DumpStrings. This attribute can be removed if we make DumpStrings smarter
            public virtual DateTime? LastModified { get; set; }

        [XmlIgnore]
        public virtual DateTime? Created { get; set; }

        public static string BuildPath(Type entityType, Guid id)
        {
            return string.Format("{0}/{1}", entityType.Name, id);
        }

        public virtual bool IsNew()
        {
            return Id == Guid.Empty;
        }

        public override string ToString()
        {
            return "{{ENTITY: {0}}}".ToFormat(Path);
        }

        //public virtual void Validate(Notification notification)
        //{
        //    if (ExtendedProperties == null) return;

        //    Validator.ValidateObject(ExtendedProperties, notification);
        //}

        public virtual object ExtendedProperties { get; set; }


    }
}