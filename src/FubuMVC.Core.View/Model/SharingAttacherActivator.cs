using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Core.View.Model
{
    public class SharingAttacherActivator<T> : IActivator where T : ITemplateFile
    {
        private readonly ITemplateRegistry<T> _templates;
        private readonly IEnumerable<ISharingAttacher<T>> _attachers;

        public SharingAttacherActivator(ITemplateRegistry<T> templates, IEnumerable<ISharingAttacher<T>> attachers)
        {
            _templates = templates;
            _attachers = attachers;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _templates.Each(t =>
            {                
                var context = new AttachRequest<T>
                {
                    Template = t,
                    Logger = TemplateLogger.Default()
                };

                _attachers.Where(a => a.CanAttach(context)).Each(a => a.Attach(context));
            });
        }
    }
}