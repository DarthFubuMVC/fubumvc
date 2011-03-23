using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Domain;
using FubuFastPack.StructureMap;
using FubuValidation;

namespace FubuFastPack.Validation
{
    public class UniqueValidationSource : IValidationSource
    {
        private readonly ITransactionProcessor _transactionProcessor;
        private readonly ITypeDescriptorCache _descriptors;
        private readonly Cache<Type, IEnumerable<IValidationRule>> _rules;

        public UniqueValidationSource(ITransactionProcessor transactionProcessor, ITypeDescriptorCache descriptors)
        {
            _transactionProcessor = transactionProcessor;
            _descriptors = descriptors;

            _rules = new Cache<Type, IEnumerable<IValidationRule>>(DetermineRulesFor);
        }

        public IEnumerable<IValidationRule> DetermineRulesFor(Type type)
        {
            if (!type.CanBeCastTo<DomainEntity>())
            {
                return new IValidationRule[0];
            }

            return _descriptors.GetPropertiesFor(type).Values
                .Where(x => x.HasAttribute<UniqueAttribute>())
                .Select(p => new {Prop = p, Key = p.GetAttribute<UniqueAttribute>().Key})
                .GroupBy(x => x.Key)
                .Select(x => new UniqueValidationRule(x.Key, type, _transactionProcessor, x.Select(o => o.Prop)));
        }

        public IEnumerable<IValidationRule> RulesFor(Type type)
        {
            return _rules[type];
        }
    }
}