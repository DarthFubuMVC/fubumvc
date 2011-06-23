using System.Collections.Generic;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View.Attachment
{
    public interface IViewAttacher : IConfigurationAction
    {
        IEnumerable<IViewFacility> Facilities { get; }
        TypePool Types { get; }
        void AddFacility(IViewFacility facility);
    }
}