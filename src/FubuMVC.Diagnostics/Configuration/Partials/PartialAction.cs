using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Configuration.Partials
{
    public class PartialAction<T>
        where T : class, IPartialModel
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