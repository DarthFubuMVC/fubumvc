using System;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public abstract class Provenance : DescribesItself
    {
        public readonly Guid Id = Guid.NewGuid();

        public abstract void Describe(Description description);


        private readonly Lazy<Description> _description;

        protected Provenance()
        {
            _description = new Lazy<Description>(() => Description.For(this));
        }

        public Description Description
        {
            get { return _description.Value; }
        }
    }
}