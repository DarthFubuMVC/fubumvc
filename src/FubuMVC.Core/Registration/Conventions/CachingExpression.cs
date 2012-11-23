using System;
using System.IO;
using FubuMVC.Core.Caching;

namespace FubuMVC.Core.Registration.Conventions
{
    public class CachingExpression
    {
        private readonly Policy _policy;

        public CachingExpression(Policy policy)
        {
            _policy = policy;
        }

        /// <summary>
        /// Applies VaryBy rule to any chain matching the criteria
        /// that is *already* marked as cached.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void VaryBy<T>() where T : IVaryBy
        {
            _policy.ModifyWith<VaryByModification<T>>();
        }
    }
}