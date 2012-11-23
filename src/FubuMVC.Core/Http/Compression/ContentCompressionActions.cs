using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Http.Compression
{
    public class ContentCompressionActions
    {
        private readonly Policy _policy;

        public ContentCompressionActions(Policy policy)
        {
            _policy = policy;
        }

        /// <summary>
        /// Applies content compression with both GZip and Deflate semantics
        /// to all matching chains (minus actions decorated with [DoNotCompress]
        /// </summary>
        public void Apply()
        {
            _policy.ModifyWith<ApplyContentCompression>();
        }

        /// <summary>
        /// Applies content compression with the specified http content encodings
        /// to all matching chains (minus actions decorated with [DoNotCompress]
        /// </summary>
        /// <param name="encodings"></param>
        public void ApplyWith(params IHttpContentEncoding[] encodings)
        {
            _policy.ModifyWith(new ApplyContentCompression(encodings));
        }
    }
}