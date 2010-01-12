using System;
using FubuMVC.Core.View;

namespace FubuMVC.Core.Registration.DSL
{
    public class ViewExpression
    {
        private readonly ViewAttacher _viewAttacher;

        public ViewExpression(ViewAttacher viewAttacher)
        {
            _viewAttacher = viewAttacher;
        }

        public ViewExpression Facility(IViewFacility facility)
        {
            _viewAttacher.AddFacility(facility);
            return this;
        }

        public ViewExpression TryToAttach(Action<ViewAttachmentStrategyExpression> configure)
        {
            var expression = new ViewAttachmentStrategyExpression(_viewAttacher);
            configure(expression);

            return this;
        }
    }
}