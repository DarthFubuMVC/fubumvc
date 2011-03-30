using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Partials
{
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