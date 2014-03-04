using System.Linq;
using FubuMVC.Core.View.Attachment;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class Default_ViewAttachment_Filters_Convention_Tester
    {
        [Test]
        public void building_a_ConfigurationGraph_with_no_modification_to_ViewAttacher_gets_you_the_default_view_attachment_filters()
        {
            var attacher = new ViewAttachmentPolicy();

            attacher.ActiveFilters.Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(ActionWithSameNameAndFolderAsViewReturnsViewModelType), typeof(ActionInSameFolderAsViewReturnsViewModelType), typeof(ActionReturnsViewModelType));
        }

        [Test]
        public void use_explicit_ViewAttachmentFilters_if_that_is_what_is_used()
        {
            var attacher = new ViewAttachmentPolicy();
            attacher.AddFilter(new ActionReturnsViewModelType());

            attacher.ActiveFilters.Select(x => x.GetType())
                            .ShouldHaveTheSameElementsAs(typeof(ActionReturnsViewModelType));
        }
    }
}