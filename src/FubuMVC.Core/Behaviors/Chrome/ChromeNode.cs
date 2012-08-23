using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuCore;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Behaviors.Chrome
{
    public class ChromeNode : BehaviorNode, DescribesItself
    {
        private readonly Type _contentType;

        public ChromeNode(Type contentType) : base()
        {
            _contentType = contentType;
        }

        protected override ObjectDef buildObjectDef()
        {
            var def = new ObjectDef(typeof (ChromeBehavior<>), _contentType);
            def.DependencyByValue<Action<ChromeContent>>(content => content.Title = Title());

            return def;
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }

        public Type ContentType
        {
            get
            {
                return _contentType;
            }
        }

        public Func<string> Title { get; set; }

        void DescribesItself.Describe(Description description)
        {
            description.Title = "Chrome / " + _contentType.Name;
            description.ShortDescription =
                "Applies 'chrome' html around the inner behavior's output by executing the chain representing the {0} model"
                    .ToFormat(_contentType.Name);


        }
    }

    
}