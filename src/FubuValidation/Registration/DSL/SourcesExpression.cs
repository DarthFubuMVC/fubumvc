using System.Collections.Generic;

namespace FubuValidation.Registration.DSL
{
    public class SourcesExpression
    {
        private readonly List<IValidationSource> _sources;

        public SourcesExpression(List<IValidationSource> sources)
        {
            _sources = sources;
        }

        public SourcesExpression AddSource<TSource>()
            where TSource : IValidationSource, new()
        {
            return AddSource(new TSource());
        }

        public SourcesExpression AddSource(IValidationSource source)
        {
            _sources.Fill(source);
            return this;
        }
    }
}