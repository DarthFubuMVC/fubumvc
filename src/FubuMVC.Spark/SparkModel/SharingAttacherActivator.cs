using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Spark.SparkModel
{
    public class SharingAttacherActivator : IActivator
    {
        private readonly ITemplateRegistry _templates;
        private readonly IEnumerable<ISharingAttacher> _attachers;

        public SharingAttacherActivator(ITemplateRegistry templates, IEnumerable<ISharingAttacher> attachers)
        {
            _templates = templates;
            _attachers = attachers;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _templates.AllTemplates().Each(t =>
            {                
                var context = new AttachRequest
                {
                    Template = t,
                    Logger = SparkLogger.Default()
                };

                _attachers.Where(a => a.CanAttach(context)).Each(a => a.Attach(context));
            });
        }
    }

    public interface IAttachRequest
    {
        ITemplate Template { get; }
        ISparkLogger Logger { get; }
    }

    public class AttachRequest : IAttachRequest
    {
        public ITemplate Template { get; set; }
        public ISparkLogger Logger { get; set; }
    }
}