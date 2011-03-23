using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.StructureMap;
using FubuValidation;
using NHibernate;
using NHibernate.Criterion;

namespace FubuFastPack.Validation
{
    public class UniqueValidationRule : IValidationRule
    {
        private readonly string _key;
        private readonly Type _targetType;
        private readonly ITransactionProcessor _transactions;
        private readonly IEnumerable<PropertyInfo> _properties;

        public UniqueValidationRule(string key, Type targetType, ITransactionProcessor transactions, IEnumerable<PropertyInfo> properties)
        {
            _key = key;
            _targetType = targetType;
            _transactions = transactions;
            _properties = properties;
        }

        public string Key
        {
            get { return _key; }
        }

        public Type TargetType
        {
            get { return _targetType; }
        }

        public IEnumerable<PropertyInfo> Properties
        {
            get { return _properties; }
        }

        public void Validate(ValidationContext context)
        {
            _transactions.Execute<ISession>(session => ValidateAgainstSession(session, context));
        }

        public void ValidateAgainstSession(ISession session, ValidationContext context)
        {
            var projection = buildProjection(session, (DomainEntity)context.Target);

            var count =  Convert.ToInt64(projection.UniqueResult());
                
            Validate(count, context);
        }

        public void Validate(long count, ValidationContext context)
        {
            if (count <= 0) return;
                
            var message = new NotificationMessage(FastPackKeys.FIELD_MUST_BE_UNIQUE);
            _properties.Each(p => message.AddAccessor(new SingleProperty(p)));

            context.Notification.RegisterMessage(message);
        }

        private ICriteria buildProjection(ISession session, DomainEntity target)
        {
            var criteria = session.CreateCriteria(_targetType);
            _properties.Each(p =>
            {
                var restriction = Restrictions.Eq(p.Name, p.GetValue(target, null));
                if (p.PropertyType == typeof(string))
                {
                    restriction = restriction.IgnoreCase();
                }
                criteria.Add(restriction);
            });

            criteria.Add(Restrictions.Not(Restrictions.Eq(Entity.IdPropertyName, target.As<DomainEntity>().Id)));
            return criteria.SetProjection(Projections.Count(_properties.First().Name));
        }
    }
    
}