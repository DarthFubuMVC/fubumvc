using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark.Tokenization;

namespace FubuMVC.Spark.Registration.Nodes
{
    public class SparkViewOutput : OutputNode<SparkViewBehavior>
    {
        private readonly SparkItem _item;
        public SparkViewOutput(SparkItem item) { _item = item; }
        
        protected override void configureObject(ObjectDef def)
        {

        }

        public override string Description
        {
            get
            {
                return string.Format("Spark View [{0}]", _item.RelativePath());
            }
        }
    }
}