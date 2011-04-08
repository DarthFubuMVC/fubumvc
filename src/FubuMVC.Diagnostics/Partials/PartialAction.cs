using System.Collections.Generic;
using FubuMVC.Core;

namespace FubuMVC.Diagnostics.Partials
{
    [FubuPartial]
    public class PartialAction<T>
        where T : class
    {
        private readonly IEnumerable<IPartialDecorator<T>> _decorators;

        public PartialAction(IEnumerable<IPartialDecorator<T>> decorators)
        {
            _decorators = decorators;
        }

        public T Execute(T input)
        {
            _decorators.Each(decorator => input = decorator.Enrich(input));
            return input;
        }
    }
}