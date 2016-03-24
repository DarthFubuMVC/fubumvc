using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.Validation.Web.UI;

namespace FubuMVC.Core.Validation.Web
{
    public interface IValidationNode
    {
        void Modify(FormRequest request);

        ValidationMode DetermineMode(IServiceLocator services, Accessor accessor);
    }

    public class ValidationNode : IValidationNode, IEnumerable<IRenderingStrategy>
    {
        public const int DefaultTimeout = 500;

        private ValidationMode _mode;
        private readonly IList<IRenderingStrategy> _strategies = new List<IRenderingStrategy>();
        private readonly IList<IValidationModePolicy> _policies = new List<IValidationModePolicy>();

        public ValidationNode()
        {
            DefaultMode(ValidationMode.Live);

            ElementTimeout = DefaultTimeout;

            RegisterPolicy(new ValidationModeAttributePolicy());
            RegisterPolicy(new AccessorRulesValidationModePolicy());
        }

        public int ElementTimeout { get; set; }

        public ValidationMode Mode
        {
            get { return _mode; }
        }

        public void DefaultMode(ValidationMode mode)
        {
            _mode = mode;
        }

        public bool IsEmpty()
        {
            return !_strategies.Any();
        }

        void IValidationNode.Modify(FormRequest request)
        {
            Each(x => x.Modify(request));
        }

        ValidationMode IValidationNode.DetermineMode(IServiceLocator services, Accessor accessor)
        {
            var policy = _policies.LastOrDefault(x => x.Matches(services, accessor));
            if (policy != null)
            {
                return policy.DetermineMode(services, accessor);
            }

            return _mode;
        }

        public void RegisterStrategy(IRenderingStrategy strategy)
        {
            _strategies.Fill(strategy);
        }

        public void RegisterPolicy(IValidationModePolicy policy)
        {
            _policies.Fill(policy);
        }

        public void Each(Action<IRenderingStrategy> action)
        {
            _strategies.Each(action);
        }

        public void Clear()
        {
            _strategies.Clear();
        }

        public IEnumerator<IRenderingStrategy> GetEnumerator()
        {
            return _strategies.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ValidationNode)obj);
        }

        protected bool Equals(ValidationNode other)
        {
            return _strategies.SequenceEqual(other._strategies);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_strategies.GetHashCode() * 397);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static ValidationNode Empty()
        {
            return new ValidationNode();
        }

        public static ValidationNode Default()
        {
            var validation = new ValidationNode();

            validation.RegisterStrategy(RenderingStrategies.Summary);
            validation.RegisterStrategy(RenderingStrategies.Highlight);

            return validation;
        }
    }
}