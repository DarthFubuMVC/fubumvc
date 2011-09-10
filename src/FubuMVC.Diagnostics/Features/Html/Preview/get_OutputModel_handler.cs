using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public class get_OutputModel_handler
    {
        private readonly IHtmlConventionsPreviewContextFactory _contextFactory;
        private readonly IEnumerable<IPreviewModelDecorator> _decorators;

        public get_OutputModel_handler(IHtmlConventionsPreviewContextFactory contextFactory, IEnumerable<IPreviewModelDecorator> decorators)
        {
            _contextFactory = contextFactory;
            _decorators = decorators;
        }

        public HtmlConventionsPreviewViewModel Execute(HtmlConventionsPreviewRequestModel request)
        {
            var context = _contextFactory.BuildFromPath(request.OutputModel);
            var model = new HtmlConventionsPreviewViewModel();

            _decorators
                .Each(d => d.Enrich(context, model));

            return model;
        }
    }
}