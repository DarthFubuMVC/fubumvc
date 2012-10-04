using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Registration.DSL
{
    public class ViewExpression
    {
        private readonly ConfigurationGraph _configuration;
        private readonly FubuRegistry _registry;

        public ViewExpression(ConfigurationGraph configuration, FubuRegistry registry)
        {
            _configuration = configuration;
            _registry = registry;
        }

        /// <summary>
        ///   Fine-tune the view attachment instead of using <see cref = "TryToAttachWithDefaultConventions" />
        /// </summary>
        public ViewExpression TryToAttach(Action<ViewsForActionFilterExpression> configure)
        {
            var expression = new ViewsForActionFilterExpression(_registry);
            configure(expression);

            return this;
        }

        /// <summary>
        ///   Configures the view attachment mechanism with default conventions:
        ///   a) by_ViewModel_and_Namespace_and_MethodName
        ///   b) by_ViewModel_and_Namespace
        ///   c) by_ViewModel
        /// </summary>
        /// <returns></returns>
        public ViewExpression TryToAttachWithDefaultConventions()
        {
            return TryToAttach(x =>
            {
                x.by_ViewModel_and_Namespace_and_MethodName();
                x.by_ViewModel_and_Namespace();
                x.by_ViewModel();
            });
        }


        /// <summary>
        ///   Define a view activation policy for views matching the filter.
        ///   <seealso cref = "IfTheInputModelOfTheViewMatches" />
        /// </summary>
        public PageActivationExpression IfTheViewMatches(Func<IViewToken, bool> filter)
        {
            return new PageActivationExpression(_registry, filter);
        }

        /// <summary>
        ///   Define a view activation policy by matching on the input type of a view.
        ///   A view activation element implements <see cref = "IPageActivationAction" /> and takes part in setting up a View instance correctly
        ///   at runtime.
        /// </summary>
        public PageActivationExpression IfTheInputModelOfTheViewMatches(Func<Type, bool> filter)
        {
            Func<IViewToken, bool> combined = viewToken => {
                return filter(viewToken.ViewModel);
            };

            return IfTheViewMatches(combined);
        }

        /// <summary>
        ///   This creates a view profile for the view attachment.  Used for scenarios like
        ///   attaching multiple views to the same chain for different devices.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "prefix"></param>
        /// <example>
        ///   Profile<IsMobile>("m.") -- where "m" would mean look for views that are named "m.something"
        /// </example>
        /// <returns></returns>
        public ViewExpression Profile<T>(string prefix) where T : IConditional
        {
            Func<IViewToken, string> naming = view =>
            {
                var name = view.Name();
                return name.Substring(prefix.Length);
            };


            _registry.AlterSettings<ViewAttachmentPolicy>(policy => {
                policy.AddProfile(typeof (T), x => x.Name().StartsWith(prefix), naming);
            });

            return this;
        }
    }
}