using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Validation.Web
{
    public interface IModelBindingErrors
    {
        void AddAnyErrors<T>(Notification notification);
    }

    public class ModelBindingErrors : IModelBindingErrors
    {
        private readonly IFubuRequest _request;

        public ModelBindingErrors(IFubuRequest request)
        {
            _request = request;
        }

        public void AddAnyErrors<T>(Notification notification)
        {
            var problems = _request.ProblemsFor<T>();
            problems.Each(problem =>
            {
                notification.RegisterMessage(problem.Property, ValidationKeys.InvalidFormat);
            });
        }
    }
}