using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class TaskHealthRequest
    {
        public Uri[] Subjects { get; set; }

        protected bool Equals(TaskHealthRequest other)
        {
            return Subjects.SequenceEqual(other.Subjects);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TaskHealthRequest) obj);
        }

        public override int GetHashCode()
        {
            return (Subjects != null ? Subjects.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("TaskHealthRequest: {0}", Subjects.Select(x => x.ToString()).Join(", "));
        }
    }
}