using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class ProvenanceChain : IEnumerable<Provenance>
    {
        public readonly Guid Id = Guid.NewGuid();

        private IEnumerable<Provenance> _chain;
        private readonly ProvenanceChain _parent;

        public ProvenanceChain(IEnumerable<Provenance> chain)
        {
            _chain = chain;
        }

        private ProvenanceChain(ProvenanceChain chain, Provenance provenance)
        {
            _parent = chain;
            _chain = chain._chain.Union(new Provenance[] {provenance}).ToArray();
        }

        public void Prepend(IEnumerable<Provenance> forebears)
        {
            _chain = forebears.Union(_chain).ToArray();
        }

        public IEnumerable<Provenance> Chain
        {
            get { return _chain; }
        }

        public ProvenanceChain Push(Provenance provenance)
        {
            return new ProvenanceChain(this, provenance);
        }

        public bool Has(Provenance provenance)
        {
            return _chain.Any(x => x.Equals(provenance));
        }

        protected bool Equals(ProvenanceChain other)
        {
            return _chain.SequenceEqual(other._chain);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProvenanceChain) obj);
        }

        public override int GetHashCode()
        {
            return (_chain != null ? _chain.GetHashCode() : 0);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Provenance> GetEnumerator()
        {
            return _chain.GetEnumerator();
        }

        public override string ToString()
        {
            return _chain.Select(x => x.ToString()).Join("/ ");
        }

        public ProvenanceChain Pop()
        {
            return _parent;
        }
    }
}