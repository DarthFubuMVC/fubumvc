using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FubuMVC.Core.Assets.Content
{
    [Serializable]
    public class TooManyBatchedTransformationsException : Exception
    {
        public TooManyBatchedTransformationsException(IEnumerable<Type> types) : base("At this time, it is invalid to use more than one batched transformation in any content plan.  Transformers:  " + types.Select(x => x.FullName).Join(", "))
        {
        }

        protected TooManyBatchedTransformationsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}